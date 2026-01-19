using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Utilities;

namespace MCS.WatchTower.WebApi.DataTransferObjects.Configurations;

public record WatchTowerMonitoringConfig
{
    public string AppId => IdBuilder.AppIdFromHostAndApp(MonitoredApp.HostName, MonitoredApp.AppName);
    public AppCreateRequest MonitoredApp { get; set; }
    public string BaseUrl { get; set; }
    public int HeartbeatIntervalSeconds { get; set; } = 30;
}
