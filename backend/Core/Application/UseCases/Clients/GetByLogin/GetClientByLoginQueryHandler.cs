using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.Clients.GetByLogin;

public class GetClientByLoginQueryHandler(IClientsService clientsService) : IQueryHandler<GetClientByLoginQuery, ClientDetailedResponse>
{
    public Task<ClientDetailedResponse> HandleAsync(GetClientByLoginQuery query, CancellationToken cancellationToken)
    {
        return clientsService.GetByLoginAsync(query.Login, cancellationToken);
    }
}
