#nullable enable
using Application.UseCases.Clients.GetByQuery;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.Abstractions.Services;

public interface IClientsService
{
    Task<PagedList<ClientResponse>> GetPagedListByQueryAsync(ClientQuery query, CancellationToken cancellationToken);
    Task<ClientDetailedResponse?> GetByGaiaAsync(string gaia, CancellationToken cancellationToken);
    Task<ClientDetailedResponse?> GetByLoginAsync(string login, CancellationToken cancellationToken);
    Task<ClientResponse> CreateAsync(ClientCreateRequest clientRequest, CancellationToken cancellationToken);
    Task UpdateAsync(string gaia, ClientUpdateRequest clientRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string gaia, CancellationToken cancellationToken);
}
