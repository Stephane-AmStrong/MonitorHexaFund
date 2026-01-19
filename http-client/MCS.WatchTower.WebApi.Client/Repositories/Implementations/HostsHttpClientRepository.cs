using MCS.WatchTower.WebApi.Client.Repositories.Contracts;
using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace MCS.WatchTower.WebApi.Client.Repositories.Implementations;

internal class HostsHttpClientRepository(HttpClient httpClient, ILogger<HostsHttpClientRepository> logger) : BaseHttpClientRepository(httpClient, logger, Endpoint, Entity), IHostsHttpClientRepository
{
    private const string Entity = "host";
    private const string Endpoint = $"api/{Entity}s";

    public Task<PagedList<HostResponse>> GetPagedListAsync(HostQueryParameters queryParameters, CancellationToken cancellationToken)
    {
        return BaseGetPagedListAsync<HostResponse>(queryParameters, cancellationToken);
    }

    public Task<HostDetailedResponse> GetByIdAsync(string hostId, CancellationToken cancellationToken)
    {
        return BaseGetByIdAsync<HostDetailedResponse>(hostId, cancellationToken);
    }

    public async Task<bool> HostExistsAsync(string hostName, CancellationToken cancellationToken)
    {
        var appQueryParameters = new HostQueryParameters
        {
            WithName = hostName,
            PageSize = 1
        };

        var matchingApps = await GetPagedListAsync(appQueryParameters, cancellationToken);

        return matchingApps.Data.Count > 0;
    }

    public Task<HostResponse> CreateAsync(HostCreateRequest createRequest, CancellationToken cancellationToken)
    {
        return BaseCreateAsync<HostCreateRequest, HostResponse>(createRequest, cancellationToken);
    }

    public Task UpdateAsync(string hostId, HostUpdateRequest hostRequest, CancellationToken cancellationToken)
    {
        return BaseUpdateAsync(hostId, hostRequest, cancellationToken);
    }

    public Task DeleteAsync(string hostId, CancellationToken cancellationToken)
    {
        return BaseDeleteAsync(hostId, cancellationToken);
    }

    protected override List<KeyValuePair<string, StringValues>> AddSpecificQueryParameters(QueryParameters query)
    {
        var specificParams = new List<KeyValuePair<string, StringValues>>();


        if (query is not HostQueryParameters hostQuery)
        {
            return specificParams;
        }

        if (!string.IsNullOrWhiteSpace(hostQuery.WithName))
        {
            specificParams.Add(KeyValuePair.Create(nameof(hostQuery.WithName), new StringValues(hostQuery.WithName)));
        }

        return specificParams;
    }
}
