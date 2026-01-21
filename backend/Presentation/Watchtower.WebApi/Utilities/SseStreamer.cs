using System.Collections.Concurrent;
using System.Text.Json;
using Application.Abstractions.Services;
using Application.Models;
using MCS.WatchTower.WebApi.DataTransferObjects;
using Microsoft.Net.Http.Headers;

namespace Watchtower.WebApi.Utilities;

public class SseStreamer<TDto>(SseStreamingOptions options, ILogger<SseStreamer<TDto>> logger) where TDto : IBaseDto
{
    private readonly ConcurrentDictionary<string, HttpContext> _clients = new();

    public async Task StreamEventsAsync(
        HttpContext httpContext,
        IEventStreamingService<TDto> service,
        CancellationToken cancellationToken)
    {
        var clientId = httpContext.Connection.Id;
        logger.LogInformation("Starting SSE stream for client {ClientId} on type {DtoType}", clientId, typeof(TDto).Name);

        _clients.TryAdd(clientId, httpContext);

        SetResponseHeaders(httpContext);

        try
        {
            while (await service.Events.WaitToReadAsync(cancellationToken))
            {
                var broadcastMessage = await service.Events.ReadAsync(cancellationToken);

                logger.LogDebug("Broadcasting event {EventType} to {ClientCount} clients", broadcastMessage.MessageType, _clients.Count);

                var eventData = new
                {
                    @event = broadcastMessage.MessageType,
                    content = broadcastMessage.Record,
                    timestamp = DateTimeOffset.UtcNow
                };

                var jsonEventData = JsonSerializer.Serialize(eventData, options.JsonOptions);

                foreach (var kvp in _clients)
                {
                    var context = kvp.Value;
                    try
                    {
                        await context.Response.WriteAsync($"data: {jsonEventData}\n\n", cancellationToken);
                        await context.Response.Body.FlushAsync(cancellationToken);
                        logger.LogTrace("Event {EventType} sent to client {ClientId}", broadcastMessage.MessageType, kvp.Key);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to send event to client {ClientId}, removing from list", kvp.Key);
                        _clients.TryRemove(kvp.Key, out _);
                    }
                }
            }

            logger.LogInformation("SSE stream completed for client {ClientId}", clientId);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("SSE stream cancelled for client {ClientId} - client disconnected", clientId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during SSE streaming for client {ClientId}", clientId);
        }
        finally
        {
            logger.LogDebug("Completing SSE response for client {ClientId}", clientId);
            _clients.TryRemove(clientId, out _);
            await httpContext.Response.CompleteAsync();
        }
    }

    private static void SetResponseHeaders(HttpContext ctx)
    {
        ctx.Response.Headers.Append(HeaderNames.ContentType, "text/event-stream");
        ctx.Response.Headers.Append(HeaderNames.CacheControl, "no-cache");
        ctx.Response.Headers.Append("Connection", "keep-alive");
    }
}
