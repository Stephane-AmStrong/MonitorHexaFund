using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.Hosts.GetAppOfHost;

public class GetAppOfHostQueryHandler(IAppsService appsService) : IQueryHandler<GetAppOfHostQuery, AppDetailedResponse?>
{
    public Task<AppDetailedResponse?> HandleAsync(GetAppOfHostQuery query, CancellationToken cancellationToken)
    {
        return appsService.GetByHostAndAppNameAsync(query.HostName, query.AppName, cancellationToken);
    }
}
