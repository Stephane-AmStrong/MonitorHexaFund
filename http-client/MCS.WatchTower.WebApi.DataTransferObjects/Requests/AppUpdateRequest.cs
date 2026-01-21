namespace MCS.WatchTower.WebApi.DataTransferObjects.Requests;

public record AppUpdateRequest: AppCreateRequest
{
    public string CronStartTime { get; init; }
    public string CronStopTime { get; init; }
}
