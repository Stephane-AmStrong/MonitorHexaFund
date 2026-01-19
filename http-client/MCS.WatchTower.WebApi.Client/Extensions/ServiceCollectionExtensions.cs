using MCS.WatchTower.WebApi.Client.Repositories.Contracts;
using MCS.WatchTower.WebApi.Client.Repositories.Implementations;
using MCS.WatchTower.WebApi.Client.Services;
using MCS.WatchTower.WebApi.DataTransferObjects.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MCS.WatchTower.WebApi.Client.Extensions;

public static class ServiceCollectionExtensions
{
    private const int DefaultHttpTimeoutSeconds = 90;

    /// <summary>
    /// Registers WatchTower HTTP client repositories and the heartbeat background service.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="monitoringConfig">Configuration delegate for <see cref="WatchTowerMonitoringConfig"/>. Can be null.</param>
    /// <param name="monitoringErrors">Callback to handle configuration validation results. Receives an empty list if configuration is valid, otherwise contains error messages.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// Configuration can be provided directly or loaded from appsettings.json.
    /// The configuration is validated immediately before service registration.
    /// </para>
    /// <para>
    /// Example usage:
    /// <code>
    /// services.AddWatchTowerHttpClientAndHeartbeatService(
    ///     options => configuration.GetSection("WatchTower").Bind(options),
    ///     errors =>
    ///     {
    ///         if (errors.Count > 0)
    ///             logger.LogError("WatchTower configuration errors: {Errors}", string.Join(" | ", errors));
    ///         else
    ///             logger.LogInformation("WatchTower configured successfully.");
    ///     });
    /// </code>
    /// </para>
    /// <para>
    /// If the configuration delegate is null or the configuration is invalid,
    /// the service will not be registered, but the application will continue to start normally.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddWatchTowerHttpClientAndHeartbeatService(this IServiceCollection services, Action<WatchTowerMonitoringConfig> monitoringConfig, Action<List<string>> monitoringErrors)
    {
        // Validate configuration immediately
        var config = new WatchTowerMonitoringConfig();
        var validator = new WatchTowerConfigurationValidator();

        if (monitoringConfig is null)
        {
            var errors = new List<string> { "WatchTower monitoring configuration delegate is null. Service will not be registered." };
            monitoringErrors(errors);
        }
        else
        {
            monitoringConfig(config);
        }

        var validationErrors = validator.Validate(config);

        // Empty list = success
        monitoringErrors(validationErrors.Count > 0 ? validationErrors : []);

        // Configuration is valid, register services
        services.AddSingleton(config);
        services.AddSingleton<IWatchTowerConfigurationValidator, WatchTowerConfigurationValidator>();

        RegisterHttpClients(services);

        services.AddHostedService<HeartbeatBackgroundService>();


        return services;
    }

    private static void RegisterHttpClients(IServiceCollection services)
    {
        void ConfigureHttpClient(IServiceProvider sp, HttpClient client)
        {
            var config = sp.GetRequiredService<WatchTowerMonitoringConfig>();

            client.BaseAddress = Uri.TryCreate(config.BaseUrl, UriKind.Absolute, out Uri baseUri) ? baseUri : null;

            client.Timeout = TimeSpan.FromSeconds(DefaultHttpTimeoutSeconds);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("MCS.WatchTower.Client/1.0");
        }

        services.AddHttpClient<IAlertsHttpClientRepository, AlertsHttpClientRepository>(ConfigureHttpClient);
        services.AddHttpClient<IAppsHttpClientRepository, AppsHttpClientRepository>(ConfigureHttpClient);
        services.AddHttpClient<IAppStatusesHttpClientRepository, AppStatusesHttpClientRepository>(ConfigureHttpClient);
        services.AddHttpClient<IClientsHttpClientRepository, ClientsHttpClientRepository>(ConfigureHttpClient);
        services.AddHttpClient<IConnectionsHttpClientRepository, ConnectionsHttpClientRepository>(ConfigureHttpClient);
        services.AddHttpClient<IHostsHttpClientRepository, HostsHttpClientRepository>(ConfigureHttpClient);
    }
}
