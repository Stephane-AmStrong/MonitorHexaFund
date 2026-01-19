#nullable enable
using System.Linq.Expressions;
using Application.UseCases.Apps.GetByQuery;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;

namespace Application.Abstractions.Services;

public interface IAppsService
{
    Task<PagedList<AppResponse>> GetPagedListByQueryAsync(AppQuery query, CancellationToken cancellationToken);
    Task<List<AppResponse>> FindByConditionAsync(Expression<Func<AppResponse, bool>> expression, CancellationToken cancellationToken);
    Task<AppDetailedResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<AppDetailedResponse?> GetByHostAndAppNameAsync(string hostName, string appName, CancellationToken cancellationToken);
    Task<AppResponse> CreateAsync(AppCreateRequest appRequest, CancellationToken cancellationToken);
    Task UpdateAsync(string id, AppUpdateRequest appRequest, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
