#nullable enable
using Application.Abstractions.Handlers;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.AppStatuses.GetByQuery;

public record GetAppStatusQuery(AppStatusQueryParameters Parameters) : IQuery<PagedList<AppStatusResponse>>;
