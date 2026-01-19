using Application.Abstractions.Handlers;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.Apps.Create;

public record CreateAppCommand(AppCreateRequest Payload) : ICommand<AppResponse>;
