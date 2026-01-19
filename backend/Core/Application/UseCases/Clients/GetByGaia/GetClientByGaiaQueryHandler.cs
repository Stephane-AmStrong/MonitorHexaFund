using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.Clients.GetByGaia;

public class GetClientByGaiaQueryHandler(IClientsService clientsService) : IQueryHandler<GetClientByGaiaQuery, ClientDetailedResponse>
{
    public Task<ClientDetailedResponse> HandleAsync(GetClientByGaiaQuery query, CancellationToken cancellationToken)
    {
        return clientsService.GetByGaiaAsync(query.Gaia, cancellationToken);
    }
}
