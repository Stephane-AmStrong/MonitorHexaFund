using Application.Abstractions.Services;
using Application.Models.FlattenedConfiguration;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Mapster;
using MCS.WatchTower.WebApi.DataTransferObjects.Utilities;
using Microsoft.Extensions.Logging;

namespace Services;

public class AppConfigurationSyncService(
    IFlatConfigurationService flatConfigurationService,
    IAppsRepository appsRepository,
    ILogger<AppConfigurationSyncService> logger) : IAppConfigurationSyncService
{
    public async Task<(long updateCount, long insertCount)> SyncAppsFromFlatConfigAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting app synchronization from flat configuration");

            var (appConfigs, existingApps) = await LoadDataConcurrentlyAsync(cancellationToken);

            if (!ValidateDataForSync(appConfigs)) return (0,0);

            var (appsToUpdate, appsToInsert) = CategorizeAppOperations(appConfigs, existingApps);

            return await ExecuteOperationsInParallelAsync(appsToUpdate, appsToInsert, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Critical error during app synchronization");
            return (0, 0);
        }
    }

    private async Task<(IList<AppConfig> configs, List<App> apps)> LoadDataConcurrentlyAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Loading app configurations and existing apps concurrently");

        var configsTask = flatConfigurationService.FindAppConfigsByIdentifiersAsync();
        var appsTask = appsRepository.FindByConditionAsync(_ => true, cancellationToken);

        await Task.WhenAll(configsTask, appsTask);
        return (await configsTask, await appsTask);
    }

    private (HashSet<App> toUpdate, HashSet<App> toInsert) CategorizeAppOperations(
        IList<AppConfig> appConfigs,
        List<App> existingApps)
    {
        logger.LogDebug("Categorizing {ConfigCount} app configurations for sync operations", appConfigs.Count);

        // Create a dictionary for faster lookups (config ID → config)
        var existingAppsByKey = existingApps
            .Where(HasValidId)
            .ToDictionary(app => app.Id, StringComparer.OrdinalIgnoreCase);

        // Log any apps with invalid IDs for further investigation
        if (existingApps.Count != existingAppsByKey.Count)
        {
            var invalidApps = existingApps.Where(s => !HasValidId(s)).ToList();
            logger.LogWarning("Found {Count} apps with invalid HostName & AppNames: {HostNameAppNames}", invalidApps.Count, string.Join(", ", invalidApps.Select(s => IdBuilder.AppIdFromHostAndApp(s.HostName, s.AppName) ?? "<null>")));
        }

        var appsToUpdate = new HashSet<App>();
        var appsToInsert = new HashSet<App>();

        var validConfigs = appConfigs.Where(HasValidId);

        foreach (var config in validConfigs)
        {
            string computedId = IdBuilder.AppIdFromHostAndApp(config.HostName, config.AppName);

            var appFromConfig = config.Adapt<App>() with
            {
                Id = computedId
            };

            if (existingAppsByKey.ContainsKey(computedId))
            {
                appsToUpdate.Add(appFromConfig);
            }
            else
            {
                appsToInsert.Add(appFromConfig);
            }
        }

        logger.LogInformation("Apps to update: {UpdateCount}, apps to insert: {InsertCount}",
            appsToUpdate.Count, appsToInsert.Count);

        return (appsToUpdate, appsToInsert);
    }

    private async Task<(long updateCount, long insertCount)> ExecuteOperationsInParallelAsync(
        HashSet<App> appsToUpdate,
        HashSet<App> appsToInsert,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Executing update and insert operations in parallel");

        // Execute both operations concurrently for maximum performance
        var updateTask = ExecuteBulkUpdateAsync(appsToUpdate, cancellationToken);
        var insertTask = ExecuteBulkInsertAsync(appsToInsert, cancellationToken);

        var results = await Task.WhenAll(updateTask, insertTask);
        var totalProcessed = results.Sum();

        LogSyncResults(results[0], results[1], totalProcessed);
        return (results[0], results[1]);
    }

    private async Task<long> ExecuteBulkUpdateAsync(HashSet<App> appsToUpdate, CancellationToken cancellationToken)
    {
        if (appsToUpdate.Count == 0) return 0;

        try
        {
            var updateIds = appsToUpdate.Select(s => s.Id).ToHashSet();
            await appsRepository.BulkUpdateAsync(updateIds, appsToUpdate, cancellationToken);

            logger.LogInformation("Successfully updated {Count} existing apps", appsToUpdate.Count);
            return appsToUpdate.Count;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update {Count} apps", appsToUpdate.Count);
            return 0;
        }
    }

    private async Task<long> ExecuteBulkInsertAsync(HashSet<App> appsToInsert, CancellationToken cancellationToken)
    {
        if (appsToInsert.Count == 0) return 0;

        try
        {
            await appsRepository.BulkInsertAsync(appsToInsert, cancellationToken);

            logger.LogInformation("Successfully inserted {Count} new apps", appsToInsert.Count);
            return appsToInsert.Count;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to insert {Count} apps", appsToInsert.Count);
            return 0;
        }
    }

    private void LogSyncResults(long updateCount, long insertCount, long totalProcessed)
    {
        if (totalProcessed == 0)
        {
            logger.LogInformation("No changes detected. All apps are up to date");
        }
        else
        {
            logger.LogInformation("App synchronization completed: {UpdateCount} updated, {InsertCount} inserted, {TotalCount} total processed",
                updateCount, insertCount, totalProcessed);
        }
    }

    // Optimized helper methods
    private static bool ValidateDataForSync(IList<AppConfig> configs)
    {
        return configs?.Count > 0;
    }

    private static bool HasValidId(AppConfig config) => !string.IsNullOrEmpty(config.Id);
    private static bool HasValidId(App app) => !string.IsNullOrEmpty(app.Id);
}
