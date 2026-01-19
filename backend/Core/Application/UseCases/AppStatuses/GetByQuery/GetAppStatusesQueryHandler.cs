#nullable enable
using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.AppStatuses.GetByQuery;

public class GetAppStatusQueryHandler(IAppStatusesService appStatusesService) : IQueryHandler<GetAppStatusQuery, PagedList<AppStatusResponse>>
{
    public Task<PagedList<AppStatusResponse>> HandleAsync(GetAppStatusQuery query, CancellationToken cancellationToken)
    {
        return appStatusesService.GetPagedListByQueryAsync(new AppStatusQuery(query.Parameters), cancellationToken);
    }
}
