using MCS.WatchTower.WebApi.DataTransferObjects.Enumerations;

namespace MCS.WatchTower.WebApi.DataTransferObjects.Responses;

public record AlertResponse : IBaseDto
{
    public string Id { get; init; }
    public string AppId { get; init; }
    public string Message { get; init; }
    public AlertSeverity Severity { get; init; }
    public int Occurrence { get; init; }
    public AlertStatus Status { get; init; }
    public DateTime OccurredAt { get; init; }
}
