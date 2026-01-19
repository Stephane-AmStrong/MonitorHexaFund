using Application.Abstractions.Handlers;
using Application.Abstractions.Services;

namespace Application.UseCases.Apps.Delete;

public class DeleteAppCommandHandler(IAppsService appsService) : ICommandHandler<DeleteAppCommand>
{
    public Task HandleAsync(DeleteAppCommand command, CancellationToken cancellationToken)
    {
        return appsService.DeleteAsync(command.Id, cancellationToken);
    }
}
