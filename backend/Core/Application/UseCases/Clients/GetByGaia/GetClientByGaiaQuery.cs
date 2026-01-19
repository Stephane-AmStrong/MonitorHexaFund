using Application.Abstractions.Handlers;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.Clients.GetByGaia;

public record GetClientByGaiaQuery(string Gaia) : IQuery<ClientDetailedResponse>;
