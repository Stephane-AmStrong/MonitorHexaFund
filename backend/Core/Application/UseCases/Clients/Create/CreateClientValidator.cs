using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;

namespace Application.UseCases.Clients.Create;

public class CreateClientValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientValidator(IClientsRepository clientsRepository, IAppsRepository appsRepository)
    {
        RuleFor(command => command.Payload.Gaia)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(CreateClientCommand.Payload.Gaia))
            .MustAsync(async (clientGaia, cancellationToken) =>
            {
                var existingClients = await clientsRepository.FindByConditionAsync(c => c.Gaia == clientGaia, cancellationToken);
                return existingClients.Count == 0;
            })
            .WithMessage(string.Format(Validation.Messages.FieldAlreadyInUseByAnother, nameof(CreateClientCommand.Payload.Gaia), Validation.Entities.Client));

        RuleFor(command => command.Payload.Login)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .OverridePropertyName(nameof(CreateClientCommand.Payload.Login))
            .MustAsync(async (clientLogin, cancellationToken) =>
            {
                var existingClients = await clientsRepository.FindByConditionAsync(c => c.Login == clientLogin, cancellationToken);
                return existingClients.Count == 0;
            })
            .WithMessage(string.Format(Validation.Messages.FieldAlreadyInUseByAnother, nameof(CreateClientCommand.Payload.Login), Validation.Entities.Client));
    }
}
