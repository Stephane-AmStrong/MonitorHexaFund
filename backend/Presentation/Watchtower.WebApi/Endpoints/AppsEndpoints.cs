using System.Text.Json;
using Application.Abstractions.Handlers;
using Application.Abstractions.Services;
using Application.UseCases.Apps.Create;
using Application.UseCases.Apps.Delete;
using Application.UseCases.Apps.GetById;
using Application.UseCases.Apps.GetByQuery;
using Application.UseCases.Apps.Update;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Watchtower.WebApi.Models;
using Watchtower.WebApi.Utilities;

namespace Watchtower.WebApi.Endpoints;
public static class AppsEndpoints
{
    public static void MapAppsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/apps")
            .WithTags("Apps");

        group.MapGet("/", GetByQueryParameters)
            .Produces<IList<AppResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetAppById)
            .Produces<AppDetailedResponse>(StatusCodes.Status200OK)
            .WithName(nameof(GetAppById));

        group.MapPost("/", CreateApp)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<AppDetailedResponse>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", DeleteApp)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", UpdateApp)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/live", async (
                HttpContext ctx,
                IEventStreamingService<AppResponse> service,
                SseStreamer streamer,
                CancellationToken cancellationToken) =>
            {
                await streamer.StreamEventsAsync(ctx, service, cancellationToken);
            })
            .WithName("AppEvents")
            .WithSummary("Stream app events");
    }

    // GET /api/apps
    private static  async Task<IResult> GetByQueryParameters(IQueryHandler<GetAppQuery, PagedList<AppResponse>> handler, [AsParameters] AppQueryParameters queryParameters, HttpResponse response, CancellationToken cancellationToken)
    {
        var appsResponse = await handler.HandleAsync(new GetAppQuery(queryParameters), cancellationToken);

        response.Headers.Append("X-Pagination", JsonSerializer.Serialize(appsResponse.MetaData));

        return Results.Ok(appsResponse);
    }

    // GET /api/apps/{id}
    private static async Task<IResult> GetAppById(IQueryHandler<GetAppByIdQuery, AppDetailedResponse?> handler, string id, CancellationToken cancellationToken)
    {
        var appResponse = await handler.HandleAsync(new GetAppByIdQuery(id), cancellationToken);
        return Results.Ok(appResponse);
    }

    // POST /api/apps
    private static async Task<IResult> CreateApp(ICommandHandler<CreateAppCommand, AppResponse> handler, AppCreateRequest appRequest, CancellationToken cancellationToken)
    {
        var appResponse = await handler.HandleAsync(new CreateAppCommand(appRequest), cancellationToken);
        return Results.CreatedAtRoute(nameof(GetAppById), new { id = appResponse.Id }, appResponse);
    }

    // DELETE /api/apps/{id}
    private static async Task<IResult> DeleteApp(ICommandHandler<DeleteAppCommand> handler, string id, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new DeleteAppCommand(id), cancellationToken);
        return Results.NoContent();
    }

    // PUT /api/apps/{id}
    private static async Task<IResult> UpdateApp(ICommandHandler<UpdateAppCommand> handler, string id, AppUpdateRequest appRequest, CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new UpdateAppCommand(id, appRequest), cancellationToken);
        return Results.NoContent();
    }
}
