using Application.Abstractions.Handlers;

namespace Application.UseCases.Apps.Delete;

public record DeleteAppCommand(string Id) : ICommand;
