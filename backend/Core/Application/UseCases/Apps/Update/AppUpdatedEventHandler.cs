using Application.Abstractions.Services;
using Application.Models;
using Application.Models.Extensions;
using Domain.Abstractions.Events;
using Domain.Entities;
using Mapster;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Apps.Update;

public class AppUpdatedEventHandler(IEventStreamingService<AppResponse> eventStreaming, ILogger<AppUpdatedEventHandler> logger) : IEventHandler<UpdatedEvent<App>>
{
    public Task Handle(UpdatedEvent<App> updatedEvent, CancellationToken cancellationToken)
    {
        eventStreaming.BroadcastAsync(new BroadcastMessage<AppResponse>(updatedEvent.Record.Adapt<AppResponse>(), updatedEvent.GetBroadcastMessageType()), cancellationToken);

        logger.LogDebug("App update event processed for {AppId} at {ProcessedAt}", updatedEvent.Record.Id, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
