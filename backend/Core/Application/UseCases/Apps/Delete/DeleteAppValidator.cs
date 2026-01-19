using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.Apps.Delete;

public class DeleteAppValidator : AbstractValidator<DeleteAppCommand>
{
    public DeleteAppValidator(IAppsRepository appsRepository)
    {
        RuleFor(command => command.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(string.Format(Validation.Messages.FieldRequired))
            .MustAsync(async (appId, cancellationToken) =>
            {
                var app = await appsRepository.GetByIdAsync(appId, cancellationToken);
                return app != null;
            }).WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.App));

    }
}
