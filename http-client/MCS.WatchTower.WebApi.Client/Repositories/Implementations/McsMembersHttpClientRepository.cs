using MCS.WatchTower.WebApi.Client.Repositories.Contracts;
using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace MCS.WatchTower.WebApi.Client.Repositories.Implementations;

internal class McsMembersHttpClientRepository(HttpClient httpClient, ILogger<McsMembersHttpClientRepository> logger) : BaseHttpClientRepository(httpClient, logger, Endpoint, Entity), IMcsMembersHttpClientRepository
{
    private const string Entity = "mcsMember";
    private const string Endpoint = $"api/{Entity}s";

    public Task<PagedList<McsMemberResponse>> GetPagedListAsync(McsMemberQueryParameters queryParameters, CancellationToken cancellationToken)
    {
        return BaseGetPagedListAsync<McsMemberResponse>(queryParameters, cancellationToken);
    }

    public Task<McsMemberDetailedResponse> GetByIdAsync(string mcsMemberId, CancellationToken cancellationToken)
    {
        return BaseGetByIdAsync<McsMemberDetailedResponse>(mcsMemberId, cancellationToken);
    }

    public Task<McsMemberResponse> CreateAsync(McsMemberCreateRequest createRequest, CancellationToken cancellationToken)
    {
        return BaseCreateAsync<McsMemberCreateRequest, McsMemberResponse>(createRequest, cancellationToken);
    }

    public Task UpdateAsync(string mcsMemberId, McsMemberUpdateRequest mcsMemberRequest, CancellationToken cancellationToken)
    {
        return BaseUpdateAsync(mcsMemberId, mcsMemberRequest, cancellationToken);
    }

    public Task DeleteAsync(string mcsMemberId, CancellationToken cancellationToken)
    {
        return BaseDeleteAsync(mcsMemberId, cancellationToken);
    }

    protected override List<KeyValuePair<string, StringValues>> AddSpecificQueryParameters(QueryParameters query)
    {
        var specificParams = new List<KeyValuePair<string, StringValues>>();


        if (query is not McsMemberQueryParameters mcsMemberQuery)
        {
            return specificParams;
        }

        if (!string.IsNullOrWhiteSpace(mcsMemberQuery.WithGaia))
        {
            specificParams.Add(KeyValuePair.Create(nameof(mcsMemberQuery.WithGaia), new StringValues(mcsMemberQuery.WithGaia)));
        }

        return specificParams;
    }
}
