using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.Alerts.Update;

public class UpdateAlertValidator : AbstractValidator<UpdateAlertCommand>
{
    public UpdateAlertValidator(IAlertsRepository alertsRepository, IAppsRepository appsRepository)
    {
        RuleFor(command => command.Payload.Severity)
            .NotNull()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(UpdateAlertCommand.Payload.Severity));

        RuleFor(command => command.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(string.Format(Validation.Messages.FieldRequired))
            .MustAsync(async (alertId, cancellationToken) =>
            {
                var alert = await alertsRepository.GetByIdAsync(alertId, cancellationToken);
                return alert is not null;
            }).WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.Alert));

        RuleFor(command => command.Payload.AppId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(UpdateAlertCommand.Payload.AppId))
            .MustAsync(async (appId, cancellationToken) =>
            {
                var app = await appsRepository.GetByIdAsync(appId, cancellationToken);
                return app is not null;
            })
            .WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.App));
    }
}
