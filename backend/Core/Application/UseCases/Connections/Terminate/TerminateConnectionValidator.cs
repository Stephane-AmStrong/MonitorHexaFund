using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.Connections.Terminate;

public class TerminateConnectionValidator : AbstractValidator<TerminateConnectionCommand>
{
    public TerminateConnectionValidator(IAppsRepository appsRepository, IClientsRepository clientsRepository)
    {
        RuleFor(command => command.Payload.AppId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(string.Format(Validation.Messages.FieldRequired))
            .MustAsync(async (appId, cancellationToken) =>
            {
                var connection = await appsRepository.GetByIdAsync(appId, cancellationToken);
                return connection != null;
            }).WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.App));

        RuleFor(command => command.Payload.ClientGaia)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(string.Format(Validation.Messages.FieldRequired))
            .MustAsync(async (clientGaia, cancellationToken) =>
            {
                var connection = await clientsRepository.GetByIdAsync(clientGaia, cancellationToken);
                return connection != null;
            }).WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.Client));
    }
}
