using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace MCS.WatchTower.WebApi.Client.Repositories.Contracts;

public interface IConnectionsHttpClientRepository
{
    Task<PagedList<ConnectionResponse>> GetPagedListAsync(ConnectionQueryParameters queryParameters, CancellationToken cancellationToken);
    Task<ConnectionDetailedResponse> GetByIdAsync(string connectionId, CancellationToken cancellationToken);
    Task<bool> ConnectionExistsAsync(string clientGaia, string appId, CancellationToken cancellationToken);
    Task<ConnectionResponse> EstablishAsync(ConnectionEstablishRequest establishRequest, CancellationToken cancellationToken);
    Task TerminateAsync(string connectionId, ConnectionTerminateRequest terminateRequest, CancellationToken cancellationToken);
}
