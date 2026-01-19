using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace MCS.WatchTower.WebApi.Client.Repositories.Contracts;

public interface IHostsHttpClientRepository
{
    Task<PagedList<HostResponse>> GetPagedListAsync(HostQueryParameters queryParameters, CancellationToken cancellationToken);
    Task<HostDetailedResponse> GetByIdAsync(string hostId, CancellationToken cancellationToken);
    Task<bool> HostExistsAsync(string hostName, CancellationToken cancellationToken);
    Task<HostResponse> CreateAsync(HostCreateRequest createRequest, CancellationToken cancellationToken);
    Task UpdateAsync(string hostId, HostUpdateRequest hostRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string hostId, CancellationToken cancellationToken);
}
