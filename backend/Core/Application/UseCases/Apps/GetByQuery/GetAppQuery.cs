#nullable enable
using Application.Abstractions.Handlers;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.UseCases.Apps.GetByQuery;

public record GetAppQuery(AppQueryParameters Parameters) : IQuery<PagedList<AppResponse>>;
