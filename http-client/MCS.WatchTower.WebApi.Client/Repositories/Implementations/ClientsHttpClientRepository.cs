using MCS.WatchTower.WebApi.Client.Repositories.Contracts;
using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace MCS.WatchTower.WebApi.Client.Repositories.Implementations;

internal class ClientsHttpClientRepository(HttpClient httpClient, ILogger<ClientsHttpClientRepository> logger) : BaseHttpClientRepository(httpClient, logger, Endpoint, Entity), IClientsHttpClientRepository
{
    private const string Entity = "client";
    private const string Endpoint = $"api/{Entity}s";

    public Task<PagedList<ClientResponse>> GetPagedListAsync(ClientQueryParameters queryParameters, CancellationToken cancellationToken)
    {
        return BaseGetPagedListAsync<ClientResponse>(queryParameters, cancellationToken);
    }

    public Task<ClientDetailedResponse> GetByGaiaAsync(string clientGaia, CancellationToken cancellationToken)
    {
        return BaseGetByIdAsync<ClientDetailedResponse>(clientGaia, cancellationToken);
    }

    public async Task<bool> ClientExistsAsync(string gaia, CancellationToken cancellationToken)
    {
        var appQueryParameters = new ClientQueryParameters
        {
            WithGaia = gaia,
            PageSize = 1
        };

        var matchingClients = await GetPagedListAsync(appQueryParameters, cancellationToken);

        return matchingClients.Data.Count > 0;
    }

    public Task<ClientResponse> CreateAsync(ClientCreateRequest createRequest, CancellationToken cancellationToken)
    {
        return BaseCreateAsync<ClientCreateRequest, ClientResponse>(createRequest, cancellationToken);
    }

    public Task UpdateAsync(string clientId, ClientUpdateRequest clientRequest, CancellationToken cancellationToken)
    {
        return BaseUpdateAsync(clientId, clientRequest, cancellationToken);
    }

    public Task DeleteAsync(string clientId, CancellationToken cancellationToken)
    {
        return BaseDeleteAsync(clientId, cancellationToken);
    }

    protected override List<KeyValuePair<string, StringValues>> AddSpecificQueryParameters(QueryParameters query)
    {
        var specificParams = new List<KeyValuePair<string, StringValues>>();

        if (query is not ClientQueryParameters clientQuery)
        {
            return specificParams;
        }

        if (!string.IsNullOrWhiteSpace(clientQuery.WithGaia))
        {
            specificParams.Add(KeyValuePair.Create(nameof(clientQuery.WithGaia), new StringValues(clientQuery.WithGaia)));
        }

        if (!string.IsNullOrWhiteSpace(clientQuery.WithLogin))
        {
            specificParams.Add(KeyValuePair.Create(nameof(clientQuery.WithLogin), new StringValues(clientQuery.WithLogin)));
        }

        return specificParams;
    }
}
