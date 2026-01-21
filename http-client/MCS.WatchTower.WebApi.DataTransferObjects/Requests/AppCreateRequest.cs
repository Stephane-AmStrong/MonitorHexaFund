namespace MCS.WatchTower.WebApi.DataTransferObjects.Requests;

public record AppCreateRequest
{
    public string HostName { get; init; }
    public string AppName { get; init; }
    public string Port { get; init; }
    public string Type { get; init; }
    public string Version { get; init; }
    public ISet<string> Tags { get; init; }
}
