using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.Apps.GetById;

public class GetAppByIdQueryHandler(IAppsService appsService) : IQueryHandler<GetAppByIdQuery, AppDetailedResponse>
{
    public Task<AppDetailedResponse> HandleAsync(GetAppByIdQuery query, CancellationToken cancellationToken)
    {
        return appsService.GetByIdAsync(query.Id, cancellationToken);
    }
}
