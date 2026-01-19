using MCS.WatchTower.WebApi.Client.Repositories.Contracts;
using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace MCS.WatchTower.WebApi.Client.Repositories.Implementations;

internal class AppStatusesHttpClientRepository(HttpClient httpClient, ILogger<AppStatusesHttpClientRepository> logger) : BaseHttpClientRepository(httpClient, logger, Endpoint, Entity), IAppStatusesHttpClientRepository
{
    private const string Entity = "app-status";
    private const string Endpoint = $"api/{Entity}es";

    public Task<PagedList<AppStatusResponse>> GetPagedListAsync(AppStatusQueryParameters queryParameters, CancellationToken cancellationToken)
    {
        return BaseGetPagedListAsync<AppStatusResponse>(queryParameters, cancellationToken);
    }

    public Task<AppStatusDetailedResponse> GetByIdAsync(string appStatusId, CancellationToken cancellationToken)
    {
        return BaseGetByIdAsync<AppStatusDetailedResponse>(appStatusId, cancellationToken);
    }

    public Task<AppStatusResponse> CreateAsync(AppStatusCreateRequest createRequest, CancellationToken cancellationToken)
    {
        return BaseCreateAsync<AppStatusCreateRequest, AppStatusResponse>(createRequest, cancellationToken);
    }

    protected override List<KeyValuePair<string, StringValues>> AddSpecificQueryParameters(QueryParameters query)
    {
        var specificParams = new List<KeyValuePair<string, StringValues>>();


        if (query is not AppStatusQueryParameters appStatusQueryParameters)
        {
            return specificParams;
        }

        if (!string.IsNullOrWhiteSpace(appStatusQueryParameters.WithAppId))
        {
            specificParams.Add(KeyValuePair.Create(nameof(appStatusQueryParameters.WithAppId), new StringValues(appStatusQueryParameters.WithAppId)));
        }

        if (appStatusQueryParameters.OfStatus is not null)
        {
            specificParams.Add(KeyValuePair.Create(nameof(appStatusQueryParameters.OfStatus), new StringValues(appStatusQueryParameters.OfStatus.ToString())));
        }

        if (appStatusQueryParameters.RecordedBefore.HasValue)
        {
            specificParams.Add(KeyValuePair.Create(nameof(appStatusQueryParameters.RecordedBefore), new StringValues(appStatusQueryParameters.RecordedBefore.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"))));
        }

        if (appStatusQueryParameters.RecordedAfter.HasValue)
        {
            specificParams.Add(KeyValuePair.Create(nameof(appStatusQueryParameters.RecordedAfter), new StringValues(appStatusQueryParameters.RecordedAfter.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"))));
        }

        return specificParams;
    }
}
