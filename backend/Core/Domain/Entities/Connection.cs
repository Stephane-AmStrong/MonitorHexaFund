namespace Domain.Entities;

public record Connection : BaseEntity
{
    public string ClientGaia { get; init; }
    public string AppId { get; init; }
    public string ProcessId { get; init; }
    public string ApiVersion { get; init; }
    public string Machine { get; init; }
    public DateTime EstablishedAt { get; init; }
    public DateTime? TerminatedAt { get; init; }
}
