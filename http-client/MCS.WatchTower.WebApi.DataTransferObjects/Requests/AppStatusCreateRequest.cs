#nullable enable
using MCS.WatchTower.WebApi.DataTransferObjects.Enumerations;

namespace MCS.WatchTower.WebApi.DataTransferObjects.Requests;

public record AppStatusCreateRequest
{
    public string? AppId { get; init; }
    public AppStatus? Status { get; init; } = AppStatus.Up;
}
