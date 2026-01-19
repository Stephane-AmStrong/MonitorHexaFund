using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.AppStatuses.Delete;

public class DeleteAppStatusValidator : AbstractValidator<DeleteAppStatusCommand>
{
    public DeleteAppStatusValidator(IAppStatusesRepository appStatusesRepository)
    {
        RuleFor(command => command.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(string.Format(Validation.Messages.FieldRequired))
            .MustAsync(async (appStatusId, cancellationToken) =>
            {
                var appStatus = await appStatusesRepository.GetByIdAsync(appStatusId, cancellationToken);
                return appStatus != null;
            }).WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.AppStatus));

    }
}
