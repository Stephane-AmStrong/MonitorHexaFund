#nullable enable
using System.Collections.Concurrent;
using Application.Abstractions.Services;
using Application.Models;
using MCS.WatchTower.WebApi.DataTransferObjects.Enumerations;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Services;

public sealed class HeartbeatListenerService(
    IServiceScopeFactory scopeFactory,
    ILogger<HeartbeatListenerService> logger) : BackgroundService, IHeartbeatListenerService
{
    private const int HeartbeatTimeoutSeconds = 35;
    private const int MonitoringInterval = 1;

    private readonly ConcurrentDictionary<string, AppTimeoutMonitor> _monitoringTasks = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("HeartbeatListenerService starting with {TimeoutSeconds}s timeout", HeartbeatTimeoutSeconds);

        try
        {
            using var scope = scopeFactory.CreateScope();
            var services = new RequiredServices(scope);

            await RunMonitoringLoopAsync(services, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("HeartbeatListenerService was cancelled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Critical error in HeartbeatListenerService");
        }
        finally
        {
            CleanupAllTasks();
            logger.LogInformation("HeartbeatListenerService stopped");
        }
    }

    public void OnAppStatusReceived(string appId, AppStatus status)
    {
        if (status != AppStatus.Up)
        {
            logger.LogDebug("Ignoring heartbeat for app {AppId} with status {Status} - only Up status triggers timeout reset", appId, status);
            return;
        }

        if (string.IsNullOrWhiteSpace(appId))
        {
            logger.LogWarning("Received heartbeat with null or empty appId");
            return;
        }

        logger.LogDebug("Heartbeat received for app {AppId} - resetting timeout", appId);

        try
        {
            ResetAppTimeout(appId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to reset timeout for app {AppId}", appId);
        }
    }

    private async Task RunMonitoringLoopAsync(RequiredServices services, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting timeout monitoring loop");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ProcessCompletedTimeoutsAsync(services, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in monitoring loop");
                await Task.Delay(TimeSpan.FromSeconds(MonitoringInterval), cancellationToken);
            }
        }

        logger.LogDebug("Timeout monitoring loop stopped");
    }

    private async Task ProcessCompletedTimeoutsAsync(RequiredServices services, CancellationToken cancellationToken)
    {
        if (!_monitoringTasks.Any())
        {
            await Task.Delay(TimeSpan.FromSeconds(MonitoringInterval), cancellationToken);
            return;
        }

        var activeTasks = _monitoringTasks.Values.Select(m => m.TimeoutTask).ToArray();
        var completedTask = await Task.WhenAny(activeTasks);

        var completedMonitor = _monitoringTasks.Values.FirstOrDefault(m => m.TimeoutTask == completedTask);
        if (completedMonitor == null)
        {
            logger.LogDebug("Completed timeout task not found in monitoring collection");
            return;
        }

        try
        {
            await completedTask; // Get the result to check for exceptions
            await HandleAppTimeout(completedMonitor.AppId, services);
        }
        catch (OperationCanceledException)
        {
            logger.LogTrace("Monitoring task cancelled for app {AppId} - heartbeat received", completedMonitor.AppId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing timeout for app {AppId}", completedMonitor.AppId);

            if (_monitoringTasks.TryRemove(completedMonitor.AppId, out var failedMonitor))
            {
                failedMonitor.Dispose();
            }
        }
    }

    private async Task HandleAppTimeout(string appId, RequiredServices services)
    {
        if (_monitoringTasks.TryRemove(appId, out var monitor))
        {
            monitor.Dispose();

            try
            {
                await MarkAppAsDownAsync(appId, services);
                logger.LogWarning("App {AppId} timed out and marked as Down", appId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to mark app {AppId} as Down after timeout", appId);
            }
        }
        else
        {
            logger.LogWarning("Timeout occurred for app {AppId} but monitor not found", appId);
        }
    }

    private void StartMonitoringApp(string appId, TimeSpan timeout)
    {
        if (string.IsNullOrWhiteSpace(appId))
        {
            logger.LogWarning("Cannot start monitoring - appId is null or empty");
            return;
        }

        var startTime = DateTimeOffset.UtcNow;
        var expectedEndTime = startTime.Add(timeout);

        var monitor = new AppTimeoutMonitor(appId, timeout, logger);
        _monitoringTasks[appId] = monitor;
    }

    private void ResetAppTimeout(string appId)
    {
        var resetTime = DateTimeOffset.UtcNow;
        var newTimeout = TimeSpan.FromSeconds(HeartbeatTimeoutSeconds);
        var newExpectedEndTime = resetTime.Add(newTimeout);

        if (!_monitoringTasks.TryGetValue(appId, out var existingMonitor))
        {
            logger.LogInformation("App {AppId} heartbeat received; next expected before {EndTime}.", appId, newExpectedEndTime);
            StartMonitoringApp(appId, newTimeout);
            return;
        }

        var oldExpectedEndTime = existingMonitor.StartTime.Add(existingMonitor.Timeout);

        // Cancel and dispose existing monitor
        existingMonitor.Cancel();
        existingMonitor.Dispose();

        // Start new monitoring
        StartMonitoringApp(appId, newTimeout);

        logger.LogInformation("App {AppId} heartbeat received; timeout rescheduled from {OldEnd} to {NewEnd}.", appId, oldExpectedEndTime, newExpectedEndTime);
    }

    private async Task MarkAppAsDownAsync(string appId, RequiredServices services)
    {
        try
        {
            var request = new AppStatusCreateRequest
            {
                AppId = appId,
                Status = AppStatus.Down
            };

            await services.AppStatuses.CreateAsync(request, CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create Down status for app {AppId}", appId);
        }
    }

    private void CleanupAllTasks()
    {
        logger.LogDebug("Cleaning up {TaskCount} monitoring tasks", _monitoringTasks.Count);

        foreach (var monitor in _monitoringTasks.Values)
        {
            try
            {
                monitor.Cancel();
                monitor.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error disposing monitor for app {AppId}", monitor.AppId);
            }
        }

        _monitoringTasks.Clear();
        logger.LogDebug("All monitoring tasks cleaned up");
    }

    private record RequiredServices(IServiceScope Scope)
    {
        public IAppsService Apps { get; } = Scope.ServiceProvider.GetRequiredService<IAppsService>();
        public IAppStatusesService AppStatuses { get; } = Scope.ServiceProvider.GetRequiredService<IAppStatusesService>();
    }
}
