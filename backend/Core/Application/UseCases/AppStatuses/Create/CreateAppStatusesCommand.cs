using Application.Abstractions.Handlers;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.AppStatuses.Create;

public record CreateAppStatusCommand(AppStatusCreateRequest Payload) : ICommand<AppStatusResponse>;
