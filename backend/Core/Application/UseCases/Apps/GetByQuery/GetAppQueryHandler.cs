#nullable enable
using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.Apps.GetByQuery;

public class GetAppQueryHandler(IAppsService appsService) : IQueryHandler<GetAppQuery, PagedList<AppResponse>>
{
    public Task<PagedList<AppResponse>> HandleAsync(GetAppQuery query, CancellationToken cancellationToken)
    {
        return appsService.GetPagedListByQueryAsync(new AppQuery(query.Parameters), cancellationToken);
    }
}
