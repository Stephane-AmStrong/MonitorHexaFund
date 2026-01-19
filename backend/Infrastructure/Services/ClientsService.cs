#nullable enable
using Application.Abstractions.Services;
using Application.UseCases.Clients.GetByQuery;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Shared.Common;
using Mapster;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;

namespace Services;

public sealed class ClientsService(
    IClientsRepository clientsRepository,
    IConnectionsRepository connectionsRepository,
    ILogger<ClientsService> logger
    ) : IClientsService
{
    public async Task<PagedList<ClientResponse>> GetPagedListByQueryAsync(ClientQuery queryParameters, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving clients with query parameters: {@QueryParameters}", queryParameters);
        var clients = await clientsRepository.GetPagedListByQueryAsync(queryParameters, cancellationToken);

        var clientResponses = clients.Adapt<List<ClientResponse>>();

        logger.LogInformation("Retrieved clients with meta data: {@MetaData}", clients.MetaData);
        return new PagedList<ClientResponse>(clientResponses, clients.MetaData);
    }

    public async Task<ClientDetailedResponse?> GetByGaiaAsync(string gaia, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving client with GAIA: {ClientGaia}", gaia);
        var client = await clientsRepository.GetByIdAsync(gaia, cancellationToken) ?? throw new ClientNotFoundException(gaia);

        List<Connection> connections = await connectionsRepository.FindByConditionAsync(connection => connection.ClientGaia == gaia, cancellationToken);
        logger.LogDebug("Found {ConnectionCount} connections for client {ClientGaia}", connections.Count, gaia);
        var clientDetailedResponse = client.Adapt<ClientDetailedResponse>();

        logger.LogInformation("Successfully retrieved client {ClientGaia} with {ConnectionCount} connections", gaia, connections.Count);
        return clientDetailedResponse with
        {
            Connections = connections.Adapt<IList<ConnectionResponse>>()
        };
    }

    public async Task<ClientDetailedResponse?> GetByLoginAsync(string login, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving client with LOGIN: {ClientLogin}", login);
        var client = (await clientsRepository.FindByConditionAsync(c => c.Login == login, cancellationToken)).FirstOrDefault() ?? throw new ClientNotFoundException(login);

        List<Connection> connections = await connectionsRepository.FindByConditionAsync(connection => connection.Id == client.Id, cancellationToken);
        logger.LogDebug("Found {ConnectionCount} connections for client {ClientLogin}", connections.Count, login);

        var clientDetailedResponse = client.Adapt<ClientDetailedResponse>();

        logger.LogInformation("Successfully retrieved client {ClientLogin} with {ConnectionCount} connections", login, connections.Count);
        return clientDetailedResponse with
        {
            Connections = connections.Adapt<IList<ConnectionResponse>>()
        };
    }

    public async Task<ClientResponse> CreateAsync(ClientCreateRequest clientRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new client with gaia: {ClientGaia}", clientRequest.Gaia);
        var client = clientRequest.Adapt<Client>() with
        {
            Id = clientRequest.Gaia.ToLowerInvariant()
        };

        await clientsRepository.CreateAsync(client, cancellationToken);

        logger.LogInformation("Successfully created client with GAIA: {ClientGaia}", client.Gaia);
        return client.Adapt<ClientResponse>();
    }

    public async Task UpdateAsync(string gaia, ClientUpdateRequest clientRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating client with GAIA: {ClientGaia}", gaia);

        var client = await clientsRepository.GetByIdAsync(gaia, cancellationToken) ?? throw new ClientNotFoundException(gaia);

        clientRequest.Adapt(client);

        logger.LogDebug("Applying updates to client {ClientGaia}: HostName={NewName}, AppName={NewAppName}", gaia, clientRequest.Gaia, clientRequest.Login);
        await clientsRepository.UpdateAsync(client, cancellationToken);

        logger.LogInformation("Successfully updated client with GAIA: {ClientGaia}", gaia);
    }

    public async Task DeleteAsync(string gaia, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting client with GAIA: {ClientGaia}", gaia);

        var client = await clientsRepository.GetByIdAsync(gaia, cancellationToken) ?? throw new ClientNotFoundException(gaia);

        await clientsRepository.DeleteAsync(client, cancellationToken);

        logger.LogInformation("Successfully deleted client with GAIA: {ClientGaia}", gaia);
    }
}
