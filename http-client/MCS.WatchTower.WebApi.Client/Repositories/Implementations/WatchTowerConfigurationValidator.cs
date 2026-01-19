using MCS.WatchTower.WebApi.Client.Repositories.Contracts;
using MCS.WatchTower.WebApi.DataTransferObjects.Configurations;

namespace MCS.WatchTower.WebApi.Client.Repositories.Implementations;

public class WatchTowerConfigurationValidator : IWatchTowerConfigurationValidator
{
    private const int MinimumHeartbeatIntervalSeconds = 30;

    public List<string> Validate(WatchTowerMonitoringConfig config)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(config.BaseUrl))
        {
            errors.Add("BaseUrl is not configured.");
        }
        else if (!Uri.TryCreate(config.BaseUrl, UriKind.Absolute, out _))
        {
            errors.Add($"BaseUrl is not a valid absolute URL: {config.BaseUrl}");
        }

        if (config.HeartbeatIntervalSeconds < MinimumHeartbeatIntervalSeconds)
        {
            errors.Add($"HeartbeatIntervalSeconds must be at least {MinimumHeartbeatIntervalSeconds} (current: {config.HeartbeatIntervalSeconds}).");
        }

        if (config.MonitoredApp is null)
        {
            errors.Add("MonitoredApp configuration is missing.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(config.MonitoredApp.HostName))
            {
                errors.Add("MonitoredApp.HostName is not configured.");
            }

            if (string.IsNullOrWhiteSpace(config.MonitoredApp.AppName))
            {
                errors.Add("MonitoredApp.AppName is not configured.");
            }
        }

        return errors;
    }
}
