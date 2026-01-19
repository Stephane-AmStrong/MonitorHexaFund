using Application.Abstractions.Handlers;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.Apps.GetById;

public record GetAppByIdQuery(string Id) : IQuery<AppDetailedResponse>;
