using Application.Abstractions.Services;
using Application.Models;
using Application.Models.Extensions;
using Domain.Abstractions.Events;
using Domain.Entities;
using Mapster;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Apps.Create;

public class AppCreatedEventHandler(IEventStreamingService<AppResponse> eventStreaming, ILogger<AppCreatedEventHandler> logger) : IEventHandler<CreatedEvent<App>>
{
    public Task Handle(CreatedEvent<App> createdEvent, CancellationToken cancellationToken)
    {
        eventStreaming.BroadcastAsync(new BroadcastMessage<AppResponse>(createdEvent.Record.Adapt<AppResponse>(), createdEvent.GetBroadcastMessageType()), cancellationToken);

        logger.LogDebug("App creation event processed for {AppId} at {ProcessedAt}", createdEvent.Record.Id, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
