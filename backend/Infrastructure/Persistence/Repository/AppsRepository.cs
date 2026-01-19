#nullable enable
using System.Linq.Expressions;
using Domain.Abstractions.Events;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Shared.Common;
using MongoDB.Driver;

namespace Persistence.Repository;

public sealed class AppsRepository(IMongoDatabase database, IEventsDispatcher eventsDispatcher) : RepositoryBase<App>(database, eventsDispatcher, DataTables.Apps), IAppsRepository
{
    public Task<PagedList<App>> GetPagedListByQueryAsync(BaseQuery<App> queryParameters, CancellationToken cancellationToken)
    {
        return BaseQueryWithFiltersAsync(queryParameters, cancellationToken);
    }

    public Task<List<App>> FindByConditionAsync(Expression<Func<App, bool>> expression, CancellationToken cancellationToken)
    {
        return BaseFindByConditionAsync(expression, cancellationToken);
    }

    public async Task<App?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var apps = await BaseFindByConditionAsync(app => app.Id == id, cancellationToken);
        return apps.FirstOrDefault();
    }

    public async Task CreateAsync(App app, CancellationToken cancellationToken)
    {
        app.Raise(new CreatedEvent<App>(app));
        await BaseCreateAsync(app, cancellationToken);
    }

    public Task UpdateAsync(App app, CancellationToken cancellationToken)
    {
        app.Raise(new UpdatedEvent<App>(app));
        return BaseUpdateAsync(app, cancellationToken);
    }

    public Task DeleteAsync(App app, CancellationToken cancellationToken)
    {
        app.Raise(new DeletedEvent<App>(app));
        return BaseDeleteAsync(app, cancellationToken);
    }

    public Task<long> BulkInsertAsync(ISet<App> updatedApps, CancellationToken cancellationToken)
    {
        foreach (var app in updatedApps)
        {
            app.Raise(new CreatedEvent<App>(app));
        }

        return BaseBulkOperationsAsync(updatedApps, BulkOperation.Insert, cancellationToken);
    }

    public Task<long> BulkUpdateAsync(ISet<string> targetIds, ISet<App> updatedApps, CancellationToken cancellationToken)
    {
        foreach (var app in updatedApps)
        {
            if (targetIds.Contains(app.Id))
            {
                app.Raise(new UpdatedEvent<App>(app));
            }
        }

        return BaseBulkOperationsAsync(updatedApps, BulkOperation.Update, cancellationToken);
    }
}
