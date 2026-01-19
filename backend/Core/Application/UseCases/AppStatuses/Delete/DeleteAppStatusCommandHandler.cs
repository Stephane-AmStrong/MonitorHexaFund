using Application.Abstractions.Handlers;
using Application.Abstractions.Services;

namespace Application.UseCases.AppStatuses.Delete;

public class DeleteAppStatusCommandHandler(IAppStatusesService appStatusesService) : ICommandHandler<DeleteAppStatusCommand>
{
    public Task HandleAsync(DeleteAppStatusCommand command, CancellationToken cancellationToken)
    {
        return appStatusesService.DeleteAsync(command.Id, cancellationToken);
    }
}
