using Application.Abstractions.Services;
using Application.Models;
using Application.Models.Extensions;
using Domain.Abstractions.Events;
using Domain.Entities;
using Mapster;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.AppStatuses.Create;

public class AppStatusCreatedEventHandler(IEventStreamingService<AppStatusResponse> eventStreaming, ILogger<AppStatusCreatedEventHandler> logger) : IEventHandler<CreatedEvent<AppStatus>>
{
    public Task Handle(CreatedEvent<AppStatus> createdEvent, CancellationToken cancellationToken)
    {
        eventStreaming.BroadcastAsync(new BroadcastMessage<AppStatusResponse>(createdEvent.Record.Adapt<AppStatusResponse>(), createdEvent.GetBroadcastMessageType()), cancellationToken);

        logger.LogDebug("AppStatus creation event processed for {AppStatusId} at {ProcessedAt}", createdEvent.Record.Id, DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
