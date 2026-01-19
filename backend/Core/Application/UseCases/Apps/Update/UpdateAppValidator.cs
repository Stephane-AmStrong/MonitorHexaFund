using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.Apps.Update;

public class UpdateAppValidator : AbstractValidator<UpdateAppCommand>
{
    public UpdateAppValidator(IAppsRepository appsRepository)
    {
        RuleFor(command => command.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(UpdateAppCommand.Id))
            .MustAsync(async (request, id, cancellationToken) =>
            {
                var existingApp = await appsRepository.GetByIdAsync(id, cancellationToken);
                return existingApp is not null;

            }).WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.App));

        RuleFor(app => app.Payload.HostName)
            .NotEmpty()
            .OverridePropertyName(nameof(UpdateAppCommand.Payload.HostName))
            .WithMessage(Validation.Messages.FieldRequired)
            .MustAsync(async (request, hostName, cancellationToken) =>
            {
                var conflictingApps = await appsRepository.FindByConditionAsync(app => app.HostName != hostName && app.Id == request.Id, cancellationToken);
                return conflictingApps.Count == 0;
            }).WithMessage(string.Format(Validation.Messages.FieldCannotBeModifiedAfterCreation, nameof(UpdateAppCommand.Payload.HostName)));

        RuleFor(app => app.Payload.AppName)
            .NotEmpty()
            .OverridePropertyName(nameof(UpdateAppCommand.Payload.AppName))
            .WithMessage(Validation.Messages.FieldRequired)
            .MustAsync(async (request, appName, cancellationToken) =>
            {
                var conflictingApps = await appsRepository.FindByConditionAsync(app => app.AppName != appName && app.Id == request.Id, cancellationToken);
                return conflictingApps.Count == 0;
            }).WithMessage(string.Format(Validation.Messages.FieldCannotBeModifiedAfterCreation, nameof(UpdateAppCommand.Payload.AppName)));
    }
}
