using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.Alerts.CreateOrIncrement;

public class CreateOrIncrementAlertValidator : AbstractValidator<CreateOrIncrementAlertCommand>
{
    public CreateOrIncrementAlertValidator(IAppsRepository appsRepository)
    {
        RuleFor(command => command.Payload.Severity)
            .NotNull()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(CreateOrIncrementAlertCommand.Payload.Severity));

        RuleFor(command => command.Payload.AppId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(CreateOrIncrementAlertCommand.Payload.AppId))
            .MustAsync(async (appId, cancellationToken) =>
            {
                var app = await appsRepository.GetByIdAsync(appId, cancellationToken);
                return app is not null;
            })
            .WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.App));
    }
}
