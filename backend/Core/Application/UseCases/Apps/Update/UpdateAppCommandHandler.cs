using Application.Abstractions.Handlers;
using Application.Abstractions.Services;

namespace Application.UseCases.Apps.Update;

public class UpdateAppCommandHandler(IAppsService appsService) : ICommandHandler<UpdateAppCommand>
{
    public Task HandleAsync(UpdateAppCommand command, CancellationToken cancellationToken)
    {
        return appsService.UpdateAsync(command.Id, command.Payload, cancellationToken);
    }
}
