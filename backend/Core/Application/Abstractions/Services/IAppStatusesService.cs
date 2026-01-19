#nullable enable
using Application.UseCases.AppStatuses.GetByQuery;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.Abstractions.Services;

public interface IAppStatusesService
{
    Task<PagedList<AppStatusResponse>> GetPagedListByQueryAsync(AppStatusQuery query, CancellationToken cancellationToken);
    Task<AppStatusDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<AppStatusResponse> CreateAsync(AppStatusCreateRequest appStatusRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
