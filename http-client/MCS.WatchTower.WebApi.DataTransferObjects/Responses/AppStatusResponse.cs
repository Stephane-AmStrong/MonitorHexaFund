using MCS.WatchTower.WebApi.DataTransferObjects.Enumerations;

namespace MCS.WatchTower.WebApi.DataTransferObjects.Responses;

public record AppStatusResponse : IBaseDto
{
    public string Id { get; init; }
    public string AppId { get; init; }
    public AppStatus Status { get; init; }
    public DateTime RecordedAt { get; init; }
}
