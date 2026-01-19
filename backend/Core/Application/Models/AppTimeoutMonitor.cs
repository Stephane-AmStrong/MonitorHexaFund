using Microsoft.Extensions.Logging;

namespace Application.Models;

public sealed record AppTimeoutMonitor : IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly ILogger _logger;
    private bool _disposed;

    public string AppId { get; }
    public DateTimeOffset StartTime { get; }
    public TimeSpan Timeout { get; }
    public Task TimeoutTask { get; }

    public AppTimeoutMonitor(string appId, TimeSpan timeout, ILogger logger)
    {
        AppId = appId;
        StartTime = DateTimeOffset.UtcNow;
        Timeout = timeout;
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();

        TimeoutTask = RunTimeoutAsync();
    }

    private async Task RunTimeoutAsync()
    {
        var expectedEndTime = StartTime.Add(Timeout);

        _logger.LogTrace("Starting {Timeout} countdown for app {AppId} - timeout at {ExpectedEndTime}",
            Timeout, AppId, expectedEndTime);

        try
        {
            await Task.Delay(Timeout, _cancellationTokenSource.Token);

            var actualEndTime = DateTimeOffset.UtcNow;
            _logger.LogDebug("Timeout reached for app {AppId} at {ActualEndTime}", AppId, actualEndTime);
        }
        catch (OperationCanceledException)
        {
            _logger.LogTrace("Timeout cancelled for app {AppId} - heartbeat received", AppId);
            throw;
        }
    }

    public void Cancel()
    {
        if (!_disposed && !_cancellationTokenSource.Token.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
            _logger.LogTrace("Cancelled timeout monitoring for app {AppId}", AppId);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _cancellationTokenSource?.Dispose();
        _disposed = true;

        _logger.LogTrace("Disposed timeout monitor for app {AppId}", AppId);
    }
}
