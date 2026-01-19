namespace Application.Abstractions.Services;

public interface IAppConfigurationSyncService
{
    Task<(long updateCount, long insertCount)> SyncAppsFromFlatConfigAsync(CancellationToken cancellationToken);
}
