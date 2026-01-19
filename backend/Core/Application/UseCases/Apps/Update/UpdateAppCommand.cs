using Application.Abstractions.Handlers;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;

namespace Application.UseCases.Apps.Update;

public record UpdateAppCommand(string Id, AppUpdateRequest Payload) : ICommand;
