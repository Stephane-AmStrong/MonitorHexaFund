using System.Text.Json;
using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using Application.UseCases.AppStatuses.Create;
using Application.UseCases.AppStatuses.Delete;
using Application.UseCases.AppStatuses.GetById;
using Application.UseCases.AppStatuses.GetByQuery;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Watchtower.WebApi.Models;
using Watchtower.WebApi.Utilities;

namespace Watchtower.WebApi.Endpoints;
public static class AppStatusesEndpoints
{
    public static void MapAppStatusesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/app-statuses")
            .WithTags("App Statuses");

        group.MapGet("/", GetByQueryParameters)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<IList<AppStatusResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetAppStatusById)
            .Produces<AppStatusDetailedResponse>(StatusCodes.Status200OK)
            .WithName(nameof(GetAppStatusById));

        group.MapPost("/", CreateAppStatus)
            .Produces<AppStatusDetailedResponse>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id}", DeleteAppStatus)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/live", async (
                HttpContext ctx,
                IEventStreamingService<AppStatusResponse> service,
                SseStreamer streamer,
                CancellationToken cancellationToken) =>
            {
                await streamer.StreamEventsAsync(ctx, service, cancellationToken);
            })
            .WithName("AppStatusEvents")
            .WithSummary("Stream app status events");
    }

    // GET /api/app-statuses
    private static  async Task<IResult> GetByQueryParameters(IQueryHandler<GetAppStatusQuery, PagedList<AppStatusResponse>> handler, [AsParameters] AppStatusQueryParameters queryParameters, HttpResponse response, CancellationToken cancellationToken)
    {
        var appStatusesResponse = await handler.HandleAsync(new GetAppStatusQuery(queryParameters), cancellationToken);

        response.Headers.Append("X-Pagination", JsonSerializer.Serialize(appStatusesResponse.MetaData));

        return Results.Ok(appStatusesResponse);
    }

    // GET /api/app-statuses/{id}
    private static async Task<IResult> GetAppStatusById(IQueryHandler<GetAppStatusByIdQuery, AppStatusDetailedResponse?> handler, string id, CancellationToken cancellationToken)
    {
        var appStatusResponse = await handler.HandleAsync(new GetAppStatusByIdQuery(id), cancellationToken);
        return Results.Ok(appStatusResponse);
    }

    // POST /api/app-statuses
    private static async Task<IResult> CreateAppStatus(ICommandHandler<CreateAppStatusCommand, AppStatusResponse> handler, AppStatusCreateRequest appStatusRequest, CancellationToken cancellationToken)
    {
        var appStatusResponse = await handler.HandleAsync(new CreateAppStatusCommand(appStatusRequest), cancellationToken);
        return Results.CreatedAtRoute(nameof(GetAppStatusById), new { id = appStatusResponse.Id }, appStatusResponse);
    }

    // DELETE /api/app-statuses/{id}
    private static async Task<IResult> DeleteAppStatus(ICommandHandler<DeleteAppStatusCommand> handler, string id, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new DeleteAppStatusCommand(id), cancellationToken);
        return Results.NoContent();
    }
}
