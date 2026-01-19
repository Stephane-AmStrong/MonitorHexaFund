namespace MCS.WatchTower.WebApi.DataTransferObjects.Requests;

public record ConnectionTerminateRequest
{
    public string AppId { get; init; }
    public string ClientGaia { get; init; }
}
