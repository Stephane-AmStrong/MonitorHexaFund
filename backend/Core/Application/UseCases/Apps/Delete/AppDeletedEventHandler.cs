using Application.Abstractions.Services;
using Application.Models;
using Application.Models.Extensions;
using Domain.Abstractions.Events;
using Domain.Entities;
using Mapster;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Apps.Delete;

public class AppDeletedEventHandler(IEventStreamingService<AppResponse> eventStreaming, ILogger<AppDeletedEventHandler> logger) : IEventHandler<DeletedEvent<App>>
{
    public Task Handle(DeletedEvent<App> deletedEvent, CancellationToken cancellationToken)
    {
        eventStreaming.BroadcastAsync(new BroadcastMessage<AppResponse>(deletedEvent.Record.Adapt<AppResponse>(), deletedEvent.GetBroadcastMessageType()), cancellationToken);

        logger.LogDebug("App deletion event processed for {AppId} at {ProcessedAt}", deletedEvent.Record.Id, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
