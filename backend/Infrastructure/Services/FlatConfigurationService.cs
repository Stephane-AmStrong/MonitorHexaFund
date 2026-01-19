#nullable enable
using Application.Abstractions.Services;
using Application.Models.FlattenedConfiguration;
using Microsoft.Extensions.Logging;

namespace Services;

public class FlatConfigurationService(ILogger<FlatConfigurationService> logger, IJsonFileReader jsonFileReader, string filePath) : IFlatConfigurationService
{
    private const int ParallelAppThreshold = 50;

    public async Task<FlatConfiguration?> LoadEnvironmentAsync()
    {
        try
        {
            FlatConfiguration? flatConfiguration = await jsonFileReader.DeserializeFileAsync<FlatConfiguration>(filePath);
            return flatConfiguration;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get FlatConfiguration from {FilePath}", filePath);
            return null;
        }
    }

    public async Task<AppConfig?> FindAppConfigByIdentifierAsync(string hostName, string alias)
    {
        IList<AppConfig> appsConfig = await FindAppConfigsByIdentifiersAsync(new List<(string hostName, string alias)> { (hostName, alias) });
        return appsConfig.FirstOrDefault();
    }

    public async Task<IList<AppConfig>> FindAppConfigsByIdentifiersAsync(IList<(string hostName, string alias)> appIdentifiers = default)
    {
        FlatConfiguration? flatConfiguration = await LoadEnvironmentAsync();
        if (flatConfiguration?.AppConfigs == null)
        {
            return Array.Empty<AppConfig>();
        }

        if (appIdentifiers is null || !appIdentifiers.Any())
        {
            return flatConfiguration.AppConfigs.ToList();
        }

        var idSet = appIdentifiers.Select(appIdentifier => $"{appIdentifier.hostName}-{appIdentifier.alias}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        bool useParallel = flatConfiguration.AppConfigs.Count >= ParallelAppThreshold;

        IEnumerable<AppConfig> query = flatConfiguration.AppConfigs.AsEnumerable();

        if (useParallel)
        {
            query = query.AsParallel();
        }

        return query
            .Where(app => app.Id is not null && idSet.Contains(app.Id))
            .ToList();
    }
}
