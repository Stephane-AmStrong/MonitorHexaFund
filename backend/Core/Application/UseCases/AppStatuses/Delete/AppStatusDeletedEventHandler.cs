using Application.Abstractions.Services;
using Application.Models;
using Application.Models.Extensions;
using Domain.Abstractions.Events;
using Domain.Entities;
using Mapster;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.AppStatuses.Delete;

public class AppStatusDeletedEventHandler(IEventStreamingService<AppStatusResponse> eventStreaming, ILogger<AppStatusDeletedEventHandler> logger) : IEventHandler<DeletedEvent<AppStatus>>
{
    public Task Handle(DeletedEvent<AppStatus> deletedEvent, CancellationToken cancellationToken)
    {
        eventStreaming.BroadcastAsync(new BroadcastMessage<AppStatusResponse>(deletedEvent.Record.Adapt<AppStatusResponse>(), deletedEvent.GetBroadcastMessageType()), cancellationToken);

        logger.LogDebug("AppStatus deletion event processed for {AppStatusId} at {ProcessedAt}", deletedEvent.Record.Id, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
