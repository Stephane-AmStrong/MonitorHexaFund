using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace MCS.WatchTower.WebApi.Client.Repositories.Contracts;

public interface IMcsMembersHttpClientRepository
{
    Task<PagedList<McsMemberResponse>> GetPagedListAsync(McsMemberQueryParameters queryParameters, CancellationToken cancellationToken);
    Task<McsMemberDetailedResponse> GetByIdAsync(string mcsMemberId, CancellationToken cancellationToken);
    Task<McsMemberResponse> CreateAsync(McsMemberCreateRequest createRequest, CancellationToken cancellationToken);
    Task UpdateAsync(string mcsMemberId, McsMemberUpdateRequest mcsMemberRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string mcsMemberId, CancellationToken cancellationToken);
}
