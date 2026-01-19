namespace Domain.Errors;

public sealed class AppNotFoundException(string id) : NotFoundException($"The app with the identifier '{id}' was not found.");
