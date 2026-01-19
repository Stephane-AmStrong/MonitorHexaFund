#nullable enable
using Application.Models.FlattenedConfiguration;

namespace Application.Abstractions.Services;

public interface IFlatConfigurationService
{
    Task<FlatConfiguration?> LoadEnvironmentAsync();
    Task<AppConfig?> FindAppConfigByIdentifierAsync(string hostName, string alias);
    Task<IList<AppConfig>> FindAppConfigsByIdentifiersAsync(IList<(string hostName, string alias)> appIdentifiers = default!);
}
