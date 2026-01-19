#nullable enable
using Application.Abstractions.Services;
using Application.UseCases.AppStatuses.GetByQuery;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Shared.Common;
using Mapster;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;

namespace Services;

public sealed class AppStatusesService(IAppStatusesRepository appStatusesRepository, IAppsRepository appsRepository, IHeartbeatListenerService heartbeatListenerService, ILogger<AppStatusesService> logger) : IAppStatusesService
{
    public async Task<PagedList<AppStatusResponse>> GetPagedListByQueryAsync(AppStatusQuery query, CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting AppStatuses with query parameters: {@QueryParameters}", query);
        var appStatuses = await appStatusesRepository.GetPagedListByQueryAsync(query, cancellationToken);

        var appStatusResponses = appStatuses.Adapt<List<AppStatusResponse>>();

        logger.LogDebug("Retrieved AppStatuses with meta data: {@MetaData}", appStatuses.MetaData);
        return new PagedList<AppStatusResponse>(appStatusResponses, appStatuses.MetaData);
    }

    public async Task<AppStatusDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Retrieving appStatus with ID: {AppStatusId}", id);
        AppStatus appStatus = await appStatusesRepository.GetByIdAsync(id, cancellationToken) ?? throw new AppStatusNotFoundException(id);

        App? app = await appsRepository.GetByIdAsync(appStatus.AppId, cancellationToken);

        if (app is null) logger.LogWarning("AppStatus {AppStatusId} refers to a missing app (AppId: {AppId})", id, appStatus.AppId);

        logger.LogDebug("AppStatus {AppStatusId} retrieved.", id);

        return appStatus.Adapt<AppStatusDetailedResponse>() with
        {
            App = app?.Adapt<AppResponse>()
        };
    }

    public async Task<AppStatusResponse> CreateAsync(AppStatusCreateRequest appStatusRequest, CancellationToken cancellationToken)
    {
        logger.LogDebug("Processing AppStatus creation for AppId={AppId} and Status={AppStatusStatus}",
            appStatusRequest.AppId, appStatusRequest.Status);

        var lastAppStatus = (await appStatusesRepository.GetLatestByAppIdsAsync(new HashSet<string>([appStatusRequest.AppId!]), cancellationToken)).FirstOrDefault();

        heartbeatListenerService.OnAppStatusReceived(appStatusRequest.AppId, appStatusRequest.Status ?? MCS.WatchTower.WebApi.DataTransferObjects.Enumerations.AppStatus.Up);

        return lastAppStatus is { Status: var lastStatus } && lastStatus == appStatusRequest.Status.ToString()
            ? lastAppStatus.Adapt<AppStatusResponse>()
            : await CreateNewAppStatus(appStatusRequest, cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting appStatus with ID: {AppStatusId}", id);

        var appStatus = await appStatusesRepository.GetByIdAsync(id, cancellationToken) ?? throw new AppStatusNotFoundException(id);

        await appStatusesRepository.DeleteAsync(appStatus, cancellationToken);

        logger.LogInformation("Successfully deleted appStatus with ID: {AppStatusId}", id);
    }

    private async Task<AppStatusResponse> CreateNewAppStatus(AppStatusCreateRequest appStatusRequest, CancellationToken cancellationToken)
    {
        logger.LogDebug("Creating new AppStatus of status: {AppStatusStatus} for appId: {AppId}", appStatusRequest.Status, appStatusRequest.AppId);

        var appStatus = appStatusRequest.Adapt<AppStatus>() with
        {
            RecordedAt = DateTime.UtcNow,
        };

        await appStatusesRepository.CreateAsync(appStatus, cancellationToken);

        logger.LogDebug("Successfully created AppStatus with ID: {AppStatusId}", appStatus.Id);

        return appStatus.Adapt<AppStatusResponse>();
    }
}
