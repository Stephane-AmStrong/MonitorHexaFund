using Application.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Services;

public class WatchtowerBootstrapService(IServiceProvider serviceProvider, ILogger<WatchtowerBootstrapService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("WatchtowerBootstrapService is starting.");

        using var scope = serviceProvider.CreateScope();

        await BootstrapAppSyncAsync(scope, stoppingToken);

        logger.LogInformation("WatchtowerBootstrapService completed successfully.");
    }

    private async Task BootstrapAppSyncAsync(IServiceScope scope, CancellationToken stoppingToken)
    {
        try
        {
            var appEnvironmentSyncService = scope.ServiceProvider.GetRequiredService<IAppConfigurationSyncService>();
            var hostEnvironmentSyncService = scope.ServiceProvider.GetRequiredService<IHostConfigurationSyncService>();

            Task<(long updateCount, long insertCount)> appEnvironmentSyncTask = appEnvironmentSyncService.SyncAppsFromFlatConfigAsync(stoppingToken);
            Task<(long updateCount, long insertCount)> hostEnvironmentSyncTask = hostEnvironmentSyncService.SyncHostsFromFlatConfigAsync(stoppingToken);

            (long updateCount, long insertCount)[] result = await Task.WhenAll(appEnvironmentSyncTask, hostEnvironmentSyncTask);

            logger.LogInformation(
                "Initial environment sync completed: {AppUpdateCount} app(s) updated, {AppInsertCount} app(s) inserted, {HostUpdateCount} host(s) updated, {HostInsertCount} host(s) inserted. Total changes: {TotalChanges}",
                result[0].updateCount, result[0].insertCount, result[1].updateCount, result[1].insertCount, result.Sum(x => x.updateCount + x.insertCount));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to perform initial app sync");
        }
    }
}
