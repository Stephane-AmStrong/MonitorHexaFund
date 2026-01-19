using Application.Common;
using Domain.Abstractions.Repositories;
using FluentValidation;
using MCS.WatchTower.WebApi.DataTransferObjects.Utilities;

namespace Application.UseCases.Connections.Establish;

public class EstablishConnectionValidator : AbstractValidator<EstablishConnectionCommand>
{
    public EstablishConnectionValidator(IConnectionsRepository connectionsRepository, IClientsRepository clientsRepository, IAppsRepository appsRepository)
    {
        RuleFor(command => command.Request.ProcessId)
            .NotEmpty()
            .OverridePropertyName(nameof(EstablishConnectionCommand.Request.ProcessId))
            .WithMessage(Validation.Messages.FieldRequired);

        RuleFor(command => command.Request.Machine)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired);

        RuleFor(command => command.Request.AppId)
            .NotEmpty()
            .WithMessage(Validation.Messages.FieldRequired)
            .MustAsync(async(appId, cancellationToken) =>
            {
                var app = await appsRepository.GetByIdAsync(appId, cancellationToken);
                return app != null;
            })
            .WithMessage(string.Format(Validation.Messages.EntityNotFound, Validation.Entities.App))
            .MustAsync(async(command, appId, cancellationToken) =>
            {
                string connectionId = IdBuilder.ConnectionIdFromAppIdAndClientGaia(appId, command.Request.ClientGaia);

                var existingConnection = await connectionsRepository.GetByIdAsync(connectionId, cancellationToken);
                return existingConnection == null;
            })
            .WithMessage(command => string.Format(Validation.Messages.RelationshipAlreadyExists, Validation.Entities.Connection, Validation.Entities.Client, command.Request.ClientGaia, Validation.Entities.App, command.Request.AppId))
            .When(command => !string.IsNullOrEmpty(command.Request.AppId) && !string.IsNullOrEmpty(command.Request.ClientGaia));
    }
}
