using Application.Abstractions.Handlers;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.Clients.GetByLogin;

public record GetClientByLoginQuery(string Login) : IQuery<ClientDetailedResponse>;
