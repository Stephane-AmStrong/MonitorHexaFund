using MCS.WatchTower.WebApi.Client.Repositories.Contracts;
using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace MCS.WatchTower.WebApi.Client.Repositories.Implementations;

internal class ConnectionsHttpClientRepository(HttpClient httpClient, ILogger<ConnectionsHttpClientRepository> logger, IClientsHttpClientRepository clientsRepository) : BaseHttpClientRepository(httpClient, logger, Endpoint, Entity), IConnectionsHttpClientRepository
{
    private const string Entity = "connection";
    private const string Endpoint = $"api/{Entity}s";

    public Task<PagedList<ConnectionResponse>> GetPagedListAsync(ConnectionQueryParameters queryParameters, CancellationToken cancellationToken)
    {
        return BaseGetPagedListAsync<ConnectionResponse>(queryParameters, cancellationToken);
    }

    public Task<ConnectionDetailedResponse> GetByIdAsync(string connectionId, CancellationToken cancellationToken)
    {
        return BaseGetByIdAsync<ConnectionDetailedResponse>(connectionId, cancellationToken);
    }

    public async Task<bool> ConnectionExistsAsync(string clientGaia, string appId, CancellationToken cancellationToken)
    {
        var appQueryParameters = new ConnectionQueryParameters
        {
            WithClientGaia = clientGaia,
            WithAppId = appId,
            PageSize = 1
        };

        var matchingApps = await GetPagedListAsync(appQueryParameters, cancellationToken);

        return matchingApps.Data.Count > 0;
    }

    public async Task<ConnectionResponse> EstablishAsync(ConnectionEstablishRequest establishRequest, CancellationToken cancellationToken)
    {
        await EnsureClientExistsAsync(establishRequest.ClientGaia, establishRequest.ClientLogin, cancellationToken);

        return await BaseCreateAsync<ConnectionEstablishRequest, ConnectionResponse>(establishRequest, cancellationToken);
    }

    public Task TerminateAsync(string connectionId, ConnectionTerminateRequest terminateRequest, CancellationToken cancellationToken)
    {
        return BaseUpdateAsync(connectionId, terminateRequest, cancellationToken);
    }

    private async Task EnsureClientExistsAsync(string clientGaia, string clientLogin, CancellationToken cancellationToken)
    {
        logger.LogInformation("Ensure client with Gaia {ClientGaia} exists", clientGaia);

        bool existingClient = await clientsRepository.ClientExistsAsync(clientGaia, cancellationToken);

        if (existingClient)
        {
            logger.LogInformation("Client with Gaia {ClientGaia} already exists", clientGaia);
            return;
        }

        logger.LogInformation("Client with Gaia '{ClientGaia}' not found. Creating new client entry", clientGaia);

        var createClientRequest = new ClientCreateRequest
        {
            Gaia = clientGaia,
            Login = clientLogin
        };

        var createdClient = await clientsRepository.CreateAsync(createClientRequest, cancellationToken);
        logger.LogInformation("Created new client with Gaia {ClientGaia}", createdClient.Gaia);
    }

    protected override List<KeyValuePair<string, StringValues>> AddSpecificQueryParameters(QueryParameters query)
    {
        var specificParams = new List<KeyValuePair<string, StringValues>>();


        if (query is not ConnectionQueryParameters connectionQuery)
        {
            return specificParams;
        }

        if (!string.IsNullOrWhiteSpace(connectionQuery.WithClientGaia))
        {
            specificParams.Add(KeyValuePair.Create(nameof(connectionQuery.WithClientGaia), new StringValues(connectionQuery.WithClientGaia)));
        }

        if (!string.IsNullOrWhiteSpace(connectionQuery.WithAppId))
        {
            specificParams.Add(KeyValuePair.Create(nameof(connectionQuery.WithAppId), new StringValues(connectionQuery.WithAppId)));
        }

        return specificParams;
    }
}
