using MCS.WatchTower.WebApi.Client.Repositories.Contracts;
using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace MCS.WatchTower.WebApi.Client.Repositories.Implementations;

internal class AppsHttpClientRepository(HttpClient httpClient, ILogger<AppsHttpClientRepository> logger) : BaseHttpClientRepository(httpClient, logger, Endpoint, Entity), IAppsHttpClientRepository
{
    private const string Entity = "app";
    private const string Endpoint = $"api/{Entity}s";

    public Task<PagedList<AppResponse>> GetPagedListAsync(AppQueryParameters queryParameters, CancellationToken cancellationToken)
    {
        return BaseGetPagedListAsync<AppResponse>(queryParameters, cancellationToken);
    }

    public Task<AppDetailedResponse> GetByIdAsync(string appId, CancellationToken cancellationToken)
    {
        return BaseGetByIdAsync<AppDetailedResponse>(appId, cancellationToken);
    }

    public async Task<bool> AppExistsAsync(string hostName, string appName, CancellationToken cancellationToken)
    {
        var appQueryParameters = new AppQueryParameters
        {
            WithHostName = hostName,
            WithAppName = appName,
            PageSize = 1
        };

        var matchingApps = await GetPagedListAsync(appQueryParameters, cancellationToken);

        return matchingApps.Data.Count > 0;
    }

    public Task<AppResponse> CreateAsync(AppCreateRequest createRequest, CancellationToken cancellationToken)
    {
        return BaseCreateAsync<AppCreateRequest, AppResponse>(createRequest, cancellationToken);
    }

    public Task UpdateAsync(string appId, AppUpdateRequest appRequest, CancellationToken cancellationToken)
    {
        return BaseUpdateAsync(appId, appRequest, cancellationToken);
    }

    public Task DeleteAsync(string appId, CancellationToken cancellationToken)
    {
        return BaseDeleteAsync(appId, cancellationToken);
    }

    protected override List<KeyValuePair<string, StringValues>> AddSpecificQueryParameters(QueryParameters query)
    {
        var specificParams = new List<KeyValuePair<string, StringValues>>();


        if (query is not AppQueryParameters appQuery)
        {
            return specificParams;
        }

        if (!string.IsNullOrWhiteSpace(appQuery.WithHostName))
        {
            specificParams.Add(KeyValuePair.Create(nameof(appQuery.WithHostName), new StringValues(appQuery.WithHostName)));
        }

        if (!string.IsNullOrWhiteSpace(appQuery.WithAppName))
        {
            specificParams.Add(KeyValuePair.Create(nameof(appQuery.WithAppName), new StringValues(appQuery.WithAppName)));
        }

        if (!string.IsNullOrEmpty(appQuery.WithVersion))
        {
            specificParams.Add(KeyValuePair.Create(nameof(appQuery.WithVersion), new StringValues(appQuery.WithVersion)));
        }

        return specificParams;
    }
}
