namespace Domain.Entities;

public record AppStatus : BaseEntity
{
    public required string AppId { get; init; }
    public required string Status { get; init; }
    public DateTime RecordedAt { get; init; }
}
