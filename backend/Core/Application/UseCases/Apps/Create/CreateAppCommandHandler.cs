using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.Apps.Create;

public class CreateAppCommandHandler(IAppsService appsService) : ICommandHandler<CreateAppCommand, AppResponse>
{
    public Task<AppResponse> HandleAsync(CreateAppCommand command, CancellationToken cancellationToken)
    {
        return appsService.CreateAsync(command.Payload, cancellationToken);
    }
}
