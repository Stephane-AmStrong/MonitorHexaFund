using MCS.WatchTower.WebApi.DataTransferObjects.Configurations;

namespace MCS.WatchTower.WebApi.Client.Repositories.Contracts;

public interface IWatchTowerConfigurationValidator
{
    List<string> Validate(WatchTowerMonitoringConfig config);
}
