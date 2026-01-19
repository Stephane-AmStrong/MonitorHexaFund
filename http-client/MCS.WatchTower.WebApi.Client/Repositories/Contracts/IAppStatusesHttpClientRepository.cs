using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace MCS.WatchTower.WebApi.Client.Repositories.Contracts;

public interface IAppStatusesHttpClientRepository
{
    Task<PagedList<AppStatusResponse>> GetPagedListAsync(AppStatusQueryParameters queryParameters, CancellationToken cancellationToken);
    Task<AppStatusDetailedResponse> GetByIdAsync(string appStatusId, CancellationToken cancellationToken);
    Task<AppStatusResponse> CreateAsync(AppStatusCreateRequest appStatusCreateRequest, CancellationToken cancellationToken);
}
