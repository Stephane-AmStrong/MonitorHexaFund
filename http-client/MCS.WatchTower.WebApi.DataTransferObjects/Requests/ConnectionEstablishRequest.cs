namespace MCS.WatchTower.WebApi.DataTransferObjects.Requests;

public record ConnectionEstablishRequest
{
    public string ClientGaia { get; init; }
    public string ClientLogin { get; init; }
    public string AppId { get; init; }
    public string ApiVersion { get; init; }
    public string Machine { get; init; }
    public string ProcessId { get; init; }
}
