using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace MCS.WatchTower.WebApi.Client.Repositories.Contracts;

public interface IAlertsHttpClientRepository
{
    Task<PagedList<AlertResponse>> GetPagedListAsync(AlertQueryParameters queryParameters, CancellationToken cancellationToken);
    Task<AlertDetailedResponse> GetByIdAsync(string alertId, CancellationToken cancellationToken);
    Task<AlertResponse> CreateOrIncrementAsync(AlertCreateOrIncrementRequest alertCreateOrIncrementRequest, CancellationToken cancellationToken);
    Task UpdateAsync(string alertId, AlertUpdateRequest alertUpdateRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string alertId, CancellationToken cancellationToken);
}
