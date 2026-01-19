using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace MCS.WatchTower.WebApi.Client.Repositories.Contracts;

public interface IClientsHttpClientRepository
{
    Task<PagedList<ClientResponse>> GetPagedListAsync(ClientQueryParameters queryParameters, CancellationToken cancellationToken);
    Task<ClientDetailedResponse> GetByGaiaAsync(string clientGaia, CancellationToken cancellationToken);
    Task<bool> ClientExistsAsync(string gaia, CancellationToken cancellationToken);
    Task<ClientResponse> CreateAsync(ClientCreateRequest clientCreateRequest, CancellationToken cancellationToken);
    Task UpdateAsync(string clientId, ClientUpdateRequest clientUpdateRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string clientId, CancellationToken cancellationToken);
}
