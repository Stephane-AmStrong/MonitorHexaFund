using MCS.WatchTower.WebApi.Client.Repositories.Contracts;
using MCS.WatchTower.WebApi.DataTransferObjects.Configurations;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MCS.WatchTower.WebApi.Client.Services;

public class HeartbeatBackgroundService(
    IAppStatusesHttpClientRepository appStatusRepository,
    IAppsHttpClientRepository appsRepository,
    IWatchTowerConfigurationValidator validator,
    ILogger<HeartbeatBackgroundService> logger,
    WatchTowerMonitoringConfig monitoringConfig)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var validationErrors = validator.Validate(monitoringConfig);

        if (validationErrors.Count > 0)
        {
            logger.LogError("WatchTower configuration is invalid. Heartbeat service will not run. Errors: {Errors}", string.Join(" | ", validationErrors));
            return;
        }

        logger.LogInformation("Heartbeat Background Service starting with interval: {Interval} seconds", monitoringConfig.HeartbeatIntervalSeconds);

        await EnsureAppExistsAsync(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendHeartbeatAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(monitoringConfig.HeartbeatIntervalSeconds), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Heartbeat Background Service is stopping");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while sending Heartbeat");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        logger.LogInformation("Heartbeat Background Service stopped");
    }

    private async Task SendHeartbeatAsync(CancellationToken cancellationToken)
    {
        var appStatusRequest = new AppStatusCreateRequest
        {
            AppId = monitoringConfig.AppId,
        };

        logger.LogDebug("Sending Heartbeat for app: {AppId} at {Timestamp}", appStatusRequest.AppId, DateTime.UtcNow);

        var response = await appStatusRepository.CreateAsync(appStatusRequest, cancellationToken);

        if (response != null)
        {
            logger.LogDebug("Heartbeat sent successfully for app: {AppId}", monitoringConfig.AppId);
        }
        else
        {
            logger.LogWarning("Failed to send Heartbeat for app: {AppId}", monitoringConfig.AppId);
        }
    }

    private async Task EnsureAppExistsAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Ensure app with ID {AppId} exists", monitoringConfig.AppId);

        var appQueryParameters = new AppQueryParameters
        {
            WithHostName = monitoringConfig.MonitoredApp.HostName,
            WithAppName = monitoringConfig.MonitoredApp.AppName,
            PageSize = 1
        };

        AppResponse existingApp = (await appsRepository.GetPagedListAsync(appQueryParameters, cancellationToken)).Data.FirstOrDefault();

        if (existingApp is not null)
        {
            logger.LogDebug("App with ID {AppId} already exists", monitoringConfig.AppId);

            var updateRequest = new AppUpdateRequest
            {
                HostName = monitoringConfig.MonitoredApp.HostName,
                AppName = monitoringConfig.MonitoredApp.AppName,
                Port = monitoringConfig.MonitoredApp.Port,
                Type = monitoringConfig.MonitoredApp.Type,
                Version = monitoringConfig.MonitoredApp.Version
            };

            await appsRepository.UpdateAsync(monitoringConfig.AppId, updateRequest, cancellationToken);

            logger.LogDebug("App with ID {AppId} updated", monitoringConfig.AppId);
        }
        else
        {
            logger.LogDebug("App with ID '{AppId}' not found. Creating new app entry for {HostName}-{AppName}", monitoringConfig.AppId, monitoringConfig.MonitoredApp.HostName, monitoringConfig.MonitoredApp.AppName);

            var createdApp = await appsRepository.CreateAsync(monitoringConfig.MonitoredApp, cancellationToken);
            logger.LogDebug("Created new app with ID {AppId}", createdApp.Id);
        }

    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Heartbeat Background Service is stopping gracefully");
        await base.StopAsync(cancellationToken);
    }
}
