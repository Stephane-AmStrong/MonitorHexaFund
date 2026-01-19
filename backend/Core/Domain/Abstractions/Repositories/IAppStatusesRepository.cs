#nullable enable
using System.Linq.Expressions;
using Domain.Entities;
using Domain.Shared.Common;

namespace Domain.Abstractions.Repositories;

public interface IAppStatusesRepository
{
    Task<PagedList<AppStatus>> GetPagedListByQueryAsync(BaseQuery<AppStatus> queryParameters, CancellationToken cancellationToken);
    Task<List<AppStatus>> FindByConditionAsync(Expression<Func<AppStatus, bool>> expression, CancellationToken cancellationToken);
    Task<List<AppStatus>> GetLatestByAppIdsAsync(ISet<string> serverIds, CancellationToken cancellationToken);
    Task<AppStatus?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task CreateAsync(AppStatus appStatus, CancellationToken cancellationToken);
    Task DeleteAsync(AppStatus appStatus, CancellationToken cancellationToken);
}
