using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.Apps.Create;

public class CreateAppValidator : AbstractValidator<CreateAppCommand>
{
    public CreateAppValidator(IAppsRepository appsRepository, IHostsRepository hostsRepository)
    {
        RuleFor(app => app.Payload.HostName)
            .NotEmpty()
            .OverridePropertyName(nameof(CreateAppCommand.Payload.HostName))
            .WithMessage(Validation.Messages.FieldRequired)
            .MustAsync(async (request, hostName, cancellationToken) =>
            {
                var conflictingApps = await appsRepository.FindByConditionAsync(app => app.HostName == hostName && app.AppName == request.Payload.AppName, cancellationToken);
                return conflictingApps.Count == 0;
            }).WithMessage(string.Format(Validation.Messages.FieldAlreadyInUseByAnother, nameof(CreateAppCommand.Payload.HostName), Validation.Entities.App));

        RuleFor(app => app.Payload.AppName)
            .NotEmpty()
            .OverridePropertyName(nameof(CreateAppCommand.Payload.AppName))
            .WithMessage(Validation.Messages.FieldRequired)
            .MustAsync(async (request, appName, cancellationToken) =>
            {
                var conflictingApps = await appsRepository.FindByConditionAsync(app => app.AppName == appName && app.HostName == request.Payload.HostName, cancellationToken);
                return conflictingApps.Count == 0;
            }).WithMessage(string.Format(Validation.Messages.FieldAlreadyInUseByAnother, nameof(CreateAppCommand.Payload.AppName), Validation.Entities.App));
    }
}
