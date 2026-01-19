#nullable enable
using System.Linq.Expressions;
using Domain.Abstractions.Events;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Shared.Common;
using MongoDB.Driver;

namespace Persistence.Repository;

public sealed class AppStatusesRepository(IMongoDatabase database, IEventsDispatcher eventsDispatcher) : RepositoryBase<AppStatus>(database, eventsDispatcher, DataTables.AppStatuses), IAppStatusesRepository
{
    public Task<PagedList<AppStatus>> GetPagedListByQueryAsync(BaseQuery<AppStatus> queryParameters, CancellationToken cancellationToken)
    {
        return BaseQueryWithFiltersAsync(queryParameters, cancellationToken);
    }

    public Task<List<AppStatus>> FindByConditionAsync(Expression<Func<AppStatus, bool>> expression, CancellationToken cancellationToken)
    {
        return BaseFindByConditionAsync(expression, cancellationToken);
    }

    public async Task<List<AppStatus>> GetLatestByAppIdsAsync(ISet<string> appIds, CancellationToken cancellationToken)
    {
        return await BaseGetLatestByGroupAsync(
            appStatus => appIds.Contains(appStatus.AppId),
            appStatus => appStatus.AppId,
            appStatus => appStatus.RecordedAt,
            cancellationToken);
    }

    public async Task<AppStatus?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var appStatuses = await BaseFindByConditionAsync(appStatus => appStatus.Id == id, cancellationToken);
        return appStatuses.FirstOrDefault();
    }

    public async Task CreateAsync(AppStatus appStatus, CancellationToken cancellationToken)
    {
        appStatus.Raise(new CreatedEvent<AppStatus>(appStatus));
        await BaseCreateAsync(appStatus, cancellationToken);
    }

    public Task DeleteAsync(AppStatus appStatus, CancellationToken cancellationToken)
    {
        appStatus.Raise(new DeletedEvent<AppStatus>(appStatus));
        return BaseDeleteAsync(appStatus, cancellationToken);
    }
}
