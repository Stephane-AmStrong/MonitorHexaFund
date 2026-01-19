#nullable enable
using System.Linq.Expressions;
using Domain.Entities;
using Domain.Shared.Common;

namespace Domain.Abstractions.Repositories;

public interface IAppsRepository
{
    Task<PagedList<App>> GetPagedListByQueryAsync(BaseQuery<App> queryParameters, CancellationToken cancellationToken);
    Task<List<App>> FindByConditionAsync(Expression<Func<App, bool>> expression, CancellationToken cancellationToken);
    Task<App?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task CreateAsync(App app, CancellationToken cancellationToken);
    Task UpdateAsync(App app, CancellationToken cancellationToken);
    Task DeleteAsync(App app, CancellationToken cancellationToken);
    Task<long> BulkInsertAsync(ISet<App> updatedApps, CancellationToken cancellationToken);
    Task<long> BulkUpdateAsync(ISet<string> targetIds, ISet<App> updatedApps, CancellationToken cancellationToken);
}
