#nullable enable
using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.Hosts.GetWithAppsByQuery;

public class GetHostWithAppsQueryHandler(IHostsService hostsService) : IQueryHandler<GetHostWithAppsQuery, PagedList<HostDetailedResponse>>
{
    public Task<PagedList<HostDetailedResponse>> HandleAsync(GetHostWithAppsQuery queryWithApps, CancellationToken cancellationToken)
    {
        return hostsService.GetPagedListWithAppsByQueryAsync(new HostWithAppsQuery(queryWithApps.Parameters), cancellationToken);
    }
}
