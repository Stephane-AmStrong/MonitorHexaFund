using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace MCS.WatchTower.WebApi.Client.Repositories.Contracts;

public interface IAppsHttpClientRepository
{
    Task<PagedList<AppResponse>> GetPagedListAsync(AppQueryParameters queryParameters, CancellationToken cancellationToken);
    Task<AppDetailedResponse> GetByIdAsync(string appId, CancellationToken cancellationToken);
    Task<AppResponse> TryGetAppAsync(string hostName, string appName, CancellationToken cancellationToken);
    Task<AppResponse> CreateAsync(AppCreateRequest createRequest, CancellationToken cancellationToken);
    Task UpdateAsync(string appId, AppUpdateRequest appRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string appId, CancellationToken cancellationToken);
}
