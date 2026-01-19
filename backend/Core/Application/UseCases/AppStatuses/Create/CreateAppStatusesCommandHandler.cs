using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.AppStatuses.Create;

public class CreateAppStatusCommandHandler(IAppStatusesService appStatusesService) : ICommandHandler<CreateAppStatusCommand, AppStatusResponse>
{
    public Task<AppStatusResponse> HandleAsync(CreateAppStatusCommand command, CancellationToken cancellationToken)
    {
        return appStatusesService.CreateAsync(command.Payload, cancellationToken);
    }
}
