#nullable enable
using Application.Abstractions.Services;
using Application.UseCases.Hosts.GetByQuery;
using Application.UseCases.Hosts.GetWithAppsByQuery;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Shared.Common;
using Mapster;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;

namespace Services;

public sealed class HostsService(IHostsRepository hostsRepository, IAppsRepository appsRepository, IAppStatusesRepository appStatusesRepository, ILogger<HostsService> logger) : IHostsService
{
    public async Task<PagedList<HostResponse>> GetPagedListByQueryAsync(HostQuery queryParameters, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving hosts with query parameters: {@QueryParameters}", queryParameters);
        var hosts = await hostsRepository.GetPagedListByQueryAsync(queryParameters, cancellationToken);

        var hostResponses = hosts.Adapt<List<HostResponse>>();

        logger.LogInformation("Retrieved hosts with meta data: {@MetaData}", hosts.MetaData);
        return new PagedList<HostResponse>(hostResponses, hosts.MetaData);
    }

    public async Task<PagedList<HostDetailedResponse>> GetPagedListWithAppsByQueryAsync(HostWithAppsQuery queryWithApps, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving hosts with query parameters: {@QueryWithApps}", queryWithApps);
        PagedList<Host> hosts = await hostsRepository.GetPagedListByQueryAsync(queryWithApps, cancellationToken);

        var hostNames = new HashSet<string>(hosts.Select(h => h.Name), StringComparer.OrdinalIgnoreCase);

        List<App> apps = await appsRepository.FindByConditionAsync(app => hostNames.Contains(app.HostName), cancellationToken);
        logger.LogDebug("Found {AppCount} apps for {HostNameCount} host names", apps.Count, hosts.Count);


        var appIds = new HashSet<string>(apps.Select(app => app.Id),  StringComparer.OrdinalIgnoreCase);
        var appStatuses = await appStatusesRepository.GetLatestByAppIdsAsync(appIds, cancellationToken);

        logger.LogDebug("Found {AppStatusCount} appStatuses for {AppIdCount} app IDs", appStatuses.Count, appIds.Count);
        var appStatusesByAppId = appStatuses.Adapt<List<AppStatusResponse>>().ToDictionary(p => p.AppId, p => p);

        var appResponsesByHostName = apps.Select(app =>
            {
                var appResponse = app.Adapt<AppResponse>();
                appStatusesByAppId.TryGetValue(app.Id, out var lastAppStatus);

                return appResponse with
                {
                    LatestStatus = lastAppStatus
                };
            }).Adapt<List<AppResponse>>()
            .GroupBy(app => app.HostName)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

        var hostResponses = hosts.AsParallel().Select(host =>
        {
            var response = host.Adapt<HostDetailedResponse>() with
            {
                Apps = appResponsesByHostName.TryGetValue(host.Name, out var list) ? list : []
            };
            return response;
        }).ToList();

        logger.LogInformation("Retrieved hosts with meta data: {@MetaData}", hosts.MetaData);
        return new PagedList<HostDetailedResponse>(hostResponses, hosts.MetaData);
    }

    public async Task<HostDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving host with ID: {HostId}", id);
        var host = await hostsRepository.GetByIdAsync(id, cancellationToken) ?? throw new HostNotFoundException(id);

        List<App> apps = await appsRepository.FindByConditionAsync(app => app.HostName == host.Name, cancellationToken);
        logger.LogDebug("Found {AppCount} apps for host {HostId}", apps.Count, id);

        var hostDetailedResponse = host.Adapt<HostDetailedResponse>();

        logger.LogInformation("Successfully retrieved host {HostId} with {AppCount} apps", id, apps.Count);
        return hostDetailedResponse with
        {
            Apps = apps.Adapt<IList<AppResponse>>()
        };
    }

    public async Task<HostDetailedResponse?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving host with NAME: {HostName}", name);
        var host = await hostsRepository.GetByNameAsync(name, cancellationToken) ?? throw new HostNotFoundException(name);

        List<App> apps = await appsRepository.FindByConditionAsync(app => app.HostName == host.Name, cancellationToken);
        logger.LogDebug("Found {AppCount} apps for host {HostName}", apps.Count, name);

        var hostDetailedResponse = host.Adapt<HostDetailedResponse>();

        logger.LogInformation("Successfully retrieved host {HostName} with {AppCount} apps", name, apps.Count);
        return hostDetailedResponse with
        {
            Apps = apps.Adapt<IList<AppResponse>>()
        };
    }

    public async Task<HostResponse> CreateAsync(HostCreateRequest hostRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new host with name: {HostName}", hostRequest.Name);
        var host = hostRequest.Adapt<Host>();

        await hostsRepository.CreateAsync(host, cancellationToken);

        logger.LogInformation("Successfully created host with ID: {HostId}", host.Id);
        return host.Adapt<HostResponse>();
    }

    public async Task<HostResponse> EnsureHostExistsAsync(string hostName, CancellationToken cancellationToken)
    {
        var existingHosts = await hostsRepository.FindByConditionAsync(x => string.Equals(x.Name, hostName, StringComparison.OrdinalIgnoreCase), cancellationToken);

        if (existingHosts.Count != 0)
        {
            var existingHost = existingHosts.First();

            logger.LogInformation("Host with name {HostName} already exists with ID: {HostId}", hostName, existingHost.Id);
            return existingHost.Adapt<HostResponse>();
        }

        logger.LogInformation("Host {HostName} does not exist. Creating host...", hostName);

        var host = new HostCreateRequest { Name = hostName }.Adapt<Host>();

        await hostsRepository.CreateAsync(host, cancellationToken);

        logger.LogInformation("Successfully created host with ID: {HostId}", host.Id);
        return host.Adapt<HostResponse>();
    }

    public async Task UpdateAsync(string id, HostUpdateRequest hostRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating host with ID: {HostId}", id);

        var host = await hostsRepository.GetByIdAsync(id, cancellationToken) ?? throw new HostNotFoundException(id);

        hostRequest.Adapt(host);

        logger.LogDebug("Applying updates to host {HostId}: HostName={NewName}", id, hostRequest.Name);
        await hostsRepository.UpdateAsync(host, cancellationToken);

        logger.LogInformation("Successfully updated host with ID: {HostId}", id);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting host with ID: {HostId}", id);

        var host = await hostsRepository.GetByIdAsync(id, cancellationToken) ?? throw new HostNotFoundException(id);

        await hostsRepository.DeleteAsync(host, cancellationToken);

        logger.LogInformation("Successfully deleted host with ID: {HostId}", id);
    }
}
