namespace MCS.WatchTower.WebApi.DataTransferObjects.Responses;

public record AppResponse : IBaseDto
{
    public string Id { get; init; }
    public string HostName { get; init; }
    public string AppName { get; init; }
    public string Port { get; init; }

    public string Type { get; init; }
    public string RunMode { get; init; }
    public AppStatusResponse LatestStatus { get; init; }

    public string CronStartTime { get; init; }
    public string CronStopTime { get; init; }

    public string Version { get; init; }
    public IList<string> Tags { get; init; }
}
