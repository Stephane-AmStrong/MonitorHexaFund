#nullable enable
using System.Linq.Expressions;
using Application.Abstractions.Services;
using Application.UseCases.AppStatuses.GetByQuery;
using Application.UseCases.Apps.GetByQuery;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Shared.Common;
using Mapster;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using MCS.WatchTower.WebApi.DataTransferObjects.Utilities;
using Microsoft.Extensions.Logging;

namespace Services;

public sealed class AppsService(IHostsService hostsService, IAppsRepository appsRepository, IConnectionsRepository connectionsRepository, IAppStatusesRepository appStatusesRepository, ILogger<AppsService> logger) : IAppsService
{
    public async Task<PagedList<AppResponse>> GetPagedListByQueryAsync(AppQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting apps with query parameters: {@QueryParameters}", query);
        var apps = await appsRepository.GetPagedListByQueryAsync(query, cancellationToken);

        var appIds = new HashSet<string>(apps.Select(app => app.Id),  StringComparer.OrdinalIgnoreCase);
        var appStatuses = await appStatusesRepository.GetLatestByAppIdsAsync(appIds, cancellationToken);

        logger.LogDebug("Found {AppStatusCount} appStatuses for {AppIdCount} app IDs", appStatuses.Count, appIds.Count);
        var appStatusesByAppId = appStatuses.ToDictionary(p => p.AppId, p => p.Adapt<AppStatusResponse>());

        var appResponses = apps.Select(app =>
        {
            var appResponse = app.Adapt<AppResponse>();
            var hasAppStatus = appStatusesByAppId.TryGetValue(app.Id, out var lastAppStatus);

            if (!hasAppStatus)
            {
                logger.LogDebug("No appStatus found for app {AppId}", app.Id);
            }

            return appResponse with
            {
                LatestStatus = lastAppStatus
            };
        }).ToList();

        var appsWithAppStatus = appResponses.Count(s => s.LatestStatus != null);

        logger.LogInformation("Retrieved {AppCount} apps ({WithAppStatusCount} with appStatus, {WithoutAppStatusCount} without appStatus). Meta data: {@MetaData}",
            appResponses.Count, appsWithAppStatus, appResponses.Count - appsWithAppStatus, apps.MetaData);

        return new PagedList<AppResponse>(appResponses, apps.MetaData);
    }

    public async Task<List<AppResponse>> FindByConditionAsync(Expression<Func<AppResponse, bool>> expression, CancellationToken cancellationToken)
    {
        logger.LogInformation("Finding apps by condition: {@Expression}", expression.ToString());

        var appExpression = ConvertExpressionToAppType(expression);

        var apps = await appsRepository.FindByConditionAsync(appExpression, cancellationToken);

        if (!apps.Any())
        {
            logger.LogInformation("No apps found matching the condition");
            return new List<AppResponse>();
        }

        var appIds = new HashSet<string>(apps.Select(app => app.Id),  StringComparer.OrdinalIgnoreCase);
        var appStatuses = await appStatusesRepository.GetLatestByAppIdsAsync(appIds, cancellationToken);

        logger.LogDebug("Found {AppStatusCount} appStatuses for {AppIdCount} app IDs",
            appStatuses.Count, appIds.Count);

        var appStatusesByAppId = appStatuses.ToDictionary(p => p.AppId, p => p.Adapt<AppStatusResponse>());

        var appResponses = apps.Select(app =>
        {
            var appResponse = app.Adapt<AppResponse>();
            var hasAppStatus = appStatusesByAppId.TryGetValue(app.Id, out var lastAppStatus);

            if (!hasAppStatus)
            {
                logger.LogDebug("No appStatus found for app {AppId}", app.Id);
            }

            return appResponse with
            {
                LatestStatus = lastAppStatus
            };
        }).ToList();

        var appsWithAppStatus = appResponses.Count(s => s.LatestStatus != null);

        logger.LogInformation("Found {AppCount} apps by condition ({WithAppStatusCount} with appStatus, {WithoutAppStatusCount} without appStatus)",
            appResponses.Count, appsWithAppStatus, appResponses.Count - appsWithAppStatus);

        return appResponses;
    }

    public async Task<AppDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving detailed app information for ID: {AppId}", id);

        App app = await appsRepository.GetByIdAsync(id, cancellationToken) ?? throw new AppNotFoundException(id);

        logger.LogDebug("App {AppId} found: {HostName} ({AppName})", id, app.HostName, app.AppName);

        var appStatusQueryParameters = new AppStatusQueryParameters(WithAppId: id, OrderBy: $"{nameof(AppStatusResponse.RecordedAt)} desc", PageSize: 10);

        var appStatusesTask = appStatusesRepository.GetPagedListByQueryAsync(new AppStatusQuery(appStatusQueryParameters), cancellationToken);
        var connectionsTask = connectionsRepository.FindByConditionAsync(connection => connection.AppId == id, cancellationToken);

        await Task.WhenAll(appStatusesTask, connectionsTask);

        PagedList<AppStatus> appStatuses = await appStatusesTask;
        List<Connection> connections = await connectionsTask;

        logger.LogDebug("Retrieved associated data for app {AppId}: {AppStatusCount} appStatuses, {ConnectionCount} connections", id, appStatuses.Count, connections.Count);

        var appResponse = app.Adapt<AppDetailedResponse>();

        logger.LogInformation("Successfully retrieved detailed app {AppId} with {ConnectionCount} connections and {AppStatusCount} recent appStatuses", id, connections.Count, appStatuses.Count);

        return appResponse with
        {
            Connections = connections.Adapt<IList<ConnectionResponse>>(),
            Statuses = appStatuses.Adapt<IList<AppStatusResponse>>()
        };
    }

    public async Task<AppDetailedResponse?> GetByHostAndAppNameAsync(string hostName, string appName, CancellationToken cancellationToken)
    {
        return await GetByIdAsync(IdBuilder.AppIdFromHostAndApp(hostName, appName), cancellationToken);
    }

    public async Task<AppResponse> CreateAsync(AppCreateRequest appRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new app with HostName: {HostName} AppName: {AppName}", appRequest.HostName, appRequest.AppName);

        await hostsService.EnsureHostExistsAsync(appRequest.HostName, cancellationToken);

        var app = appRequest.Adapt<App>() with
        {
            Id = IdBuilder.AppIdFromHostAndApp(appRequest.HostName, appRequest.AppName)
        };

        await appsRepository.CreateAsync(app, cancellationToken);

        logger.LogInformation("Successfully created app with ID: {AppId}", app.Id);

        return app.Adapt<AppResponse>();
    }

    public async Task UpdateAsync(string id, AppUpdateRequest appRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating app with ID: {AppId}", id);

        var app = await appsRepository.GetByIdAsync(id, cancellationToken) ?? throw new AppNotFoundException(id);

        appRequest.Adapt(app);

        logger.LogDebug("Applying updates to app {AppId}: HostName={NewName}, AppName={NewAppName}", id, appRequest.HostName, appRequest.AppName);
        await appsRepository.UpdateAsync(app, cancellationToken);

        logger.LogInformation("Successfully updated app with ID: {AppId}", id);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting app with ID: {AppId}", id);

        var app = await appsRepository.GetByIdAsync(id, cancellationToken) ?? throw new AppNotFoundException(id);

        await appsRepository.DeleteAsync(app, cancellationToken);

        logger.LogInformation("Successfully deleted app with ID: {AppId}", id);
    }

    private static Expression<Func<App, bool>> ConvertExpressionToAppType(Expression<Func<AppResponse, bool>> expression)
    {
        var parameter = Expression.Parameter(typeof(App), "app");

        var visitor = new ParameterReplacerVisitor(expression.Parameters[0], parameter);
        var body = visitor.Visit(expression.Body);

        return Expression.Lambda<Func<App, bool>>(body, parameter);
    }

    private class ParameterReplacerVisitor(ParameterExpression oldParameter, ParameterExpression newParameter) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == oldParameter ? newParameter : base.VisitParameter(node);
        }
    }
}
