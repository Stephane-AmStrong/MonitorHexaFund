#nullable enable
using MCS.WatchTower.WebApi.DataTransferObjects.Enumerations;

namespace MCS.WatchTower.WebApi.DataTransferObjects.Requests;

public record AlertCreateOrIncrementRequest
{
    public string? AppId { get; init; }
    public string? Message { get; init; }
    public AlertSeverity? Severity { get; init; }
}
