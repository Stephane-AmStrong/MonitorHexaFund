using Application.Abstractions.Handlers;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;

namespace Application.UseCases.Connections.Terminate;

public record TerminateConnectionCommand(string Id, ConnectionTerminateRequest Payload) : ICommand;
