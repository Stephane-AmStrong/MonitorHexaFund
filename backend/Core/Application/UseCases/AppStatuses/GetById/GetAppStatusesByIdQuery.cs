using Application.Abstractions.Handlers;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.AppStatuses.GetById;

public record GetAppStatusByIdQuery(string Id) : IQuery<AppStatusDetailedResponse>;
