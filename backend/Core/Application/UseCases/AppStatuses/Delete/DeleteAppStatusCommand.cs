using Application.Abstractions.Handlers;

namespace Application.UseCases.AppStatuses.Delete;

public record DeleteAppStatusCommand(string Id) : ICommand;
