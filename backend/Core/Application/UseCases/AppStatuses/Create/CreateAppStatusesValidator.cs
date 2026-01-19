using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.AppStatuses.Create;

public class CreateAppStatusValidator : AbstractValidator<CreateAppStatusCommand>
{
    public CreateAppStatusValidator(IAppsRepository appsRepository)
    {
        RuleFor(command => command.Payload.Status)
            .NotNull()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(CreateAppStatusCommand.Payload.Status));

        RuleFor(command => command.Payload.AppId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(CreateAppStatusCommand.Payload.AppId))
            .MustAsync(async (appId, cancellationToken) =>
            {
                var app = await appsRepository.GetByIdAsync(appId, cancellationToken);
                return app is not null;
            })
            .WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.App));
    }
}
