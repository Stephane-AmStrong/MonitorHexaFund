#nullable enable
using Application.Abstractions.Services;
using Application.UseCases.Connections.GetByQuery;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Errors;
using Domain.Shared.Common;
using Mapster;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;

namespace Services;

public sealed class ConnectionsService(
    IConnectionsRepository connectionsRepository,
    IAppsRepository appsRepository,
    IClientsRepository clientsRepository,
    ILogger<ConnectionsService> logger
    ) : IConnectionsService
{
    public async Task<PagedList<ConnectionResponse>> GetPagedListByQueryAsync(ConnectionQuery queryParameters, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving connections with query parameters: {@QueryParameters}", queryParameters);
        var connections = await connectionsRepository.GetPagedListByQueryAsync(queryParameters, cancellationToken);

        var connectionResponses = connections.Adapt<List<ConnectionResponse>>();

        logger.LogInformation("Retrieved connections with meta data: {@MetaData}", connections.MetaData);
        return new PagedList<ConnectionResponse>(connectionResponses, connections.MetaData);
    }

    public async Task<ConnectionDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving connection with ID: {ConnectionId}", id);
        var connection = await connectionsRepository.GetByIdAsync(id, cancellationToken) ?? throw new ConnectionNotFoundException(id);

        var app = await appsRepository.GetByIdAsync(connection.AppId, cancellationToken);
        var client = await clientsRepository.GetByIdAsync(connection.ClientGaia, cancellationToken);

        if (app is null) logger.LogWarning("Connection {ConnectionId} refers to a missing app (AppId: {AppId})", id, connection.AppId);
        if (client is null) logger.LogWarning("Connection {ConnectionId} refers to a missing client (ClientGaia: {ClientGaia})", id, connection.ClientGaia);

        var connectionDetailedResponse = connection.Adapt<ConnectionDetailedResponse>();

        logger.LogInformation("Successfully retrieved connection {ConnectionId} with app and client references", id);

        return connectionDetailedResponse with
        {
            App = app.Adapt<AppResponse>(),
            Client = client.Adapt<ClientResponse>()
        };
    }

    /*
    public async Task<ConnectionResponse> EstablishAsync(ConnectionEstablishRequest connectionRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Establishing new connection with appId: {AppId} and clientId: {ClientId}", connectionRequest.AppId, connectionRequest.ClientGaia);

        var existingConnection = await FindExistingActiveConnection(connectionRequest, cancellationToken);

        return existingConnection != null
                ? existingConnection.Adapt<ConnectionResponse>()
                : await CreateNewConnectionAsync(connectionRequest, cancellationToken);
    }
    */

    public async Task<ConnectionResponse> EstablishAsync(ConnectionEstablishRequest connectionRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Establishing new connection with appId: {AppId} and clientId: {ClientId}", connectionRequest.AppId, connectionRequest.ClientGaia);

        var connection = connectionRequest.Adapt<Connection>() with
        {
            AppId = connectionRequest.AppId,
            EstablishedAt = DateTime.UtcNow,
            ClientGaia = connectionRequest.ClientGaia,
        };

        await connectionsRepository.EstablishAsync(connection, cancellationToken);

        logger.LogInformation("Successfully established connection with ID: {ConnectionId}", connection.Id);
        return connection.Adapt<ConnectionResponse>();
    }

    public async Task TerminateAsync(string id, ConnectionTerminateRequest connectionRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Terminating connection with ID: {ConnectionId}", id);

        var connection = await connectionsRepository.GetByIdAsync(id, cancellationToken) ?? throw new ConnectionNotFoundException(id);

        connection = connection with { TerminatedAt = DateTime.UtcNow };

        await connectionsRepository.TerminateAsync(connection, cancellationToken);

        logger.LogInformation("Successfully terminated connection with ID: {ConnectionId}", id);
    }

    private async Task<Connection?> FindExistingActiveConnection(ConnectionEstablishRequest connectionRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking for existing connection with AppId={AppId} ClientGaia={ClientGaia}", connectionRequest.AppId, connectionRequest.ClientGaia);

        List<Connection> connections = await connectionsRepository.FindByConditionAsync
        (
            connection => connection.AppId == connectionRequest.AppId &&  connection.ClientGaia == connectionRequest.ClientGaia &&  connection.TerminatedAt == null,
            cancellationToken
        );

        return connections.FirstOrDefault();
    }

    private async Task<ConnectionResponse> CreateNewConnectionAsync(ConnectionEstablishRequest connectionRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Establishing new connection with appId: {AppId} and clientId: {ClientId}", connectionRequest.AppId, connectionRequest.ClientGaia);

        var connection = connectionRequest.Adapt<Connection>() with
        {
            AppId = connectionRequest.AppId,
            EstablishedAt = DateTime.UtcNow,
            ClientGaia = connectionRequest.ClientGaia,
        };

        await connectionsRepository.EstablishAsync(connection, cancellationToken);

        logger.LogInformation("Successfully established connection with ID: {ConnectionId}", connection.Id);
        return connection.Adapt<ConnectionResponse>();
    }
}
