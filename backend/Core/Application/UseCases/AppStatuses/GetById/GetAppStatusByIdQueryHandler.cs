using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.AppStatuses.GetById;

public class GetAppStatusByIdQueryHandler(IAppStatusesService appStatusesService) : IQueryHandler<GetAppStatusByIdQuery, AppStatusDetailedResponse>
{
    public Task<AppStatusDetailedResponse> HandleAsync(GetAppStatusByIdQuery query, CancellationToken cancellationToken)
    {
        return appStatusesService.GetByIdAsync(query.Id, cancellationToken);
    }
}
