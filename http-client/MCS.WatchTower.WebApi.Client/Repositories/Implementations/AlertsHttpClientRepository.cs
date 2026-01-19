using MCS.WatchTower.WebApi.Client.Repositories.Contracts;
using MCS.WatchTower.WebApi.DataTransferObjects.Paging;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;
using MCS.WatchTower.WebApi.DataTransferObjects.Requests;
using MCS.WatchTower.WebApi.DataTransferObjects.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace MCS.WatchTower.WebApi.Client.Repositories.Implementations;

internal class AlertsHttpClientRepository(HttpClient httpClient, ILogger<AlertsHttpClientRepository> logger) : BaseHttpClientRepository(httpClient, logger, Endpoint, Entity), IAlertsHttpClientRepository
{
    private const string Entity = "alert";
    private const string Endpoint = $"api/{Entity}s";

    public Task<PagedList<AlertResponse>> GetPagedListAsync(AlertQueryParameters queryParameters, CancellationToken cancellationToken)
    {
        return BaseGetPagedListAsync<AlertResponse>(queryParameters, cancellationToken);
    }

    public Task<AlertDetailedResponse> GetByIdAsync(string alertId, CancellationToken cancellationToken)
    {
        return BaseGetByIdAsync<AlertDetailedResponse>(alertId, cancellationToken);
    }

    public Task<AlertResponse> CreateOrIncrementAsync(AlertCreateOrIncrementRequest createOrIncrementRequest, CancellationToken cancellationToken)
    {
        return BaseCreateAsync<AlertCreateOrIncrementRequest, AlertResponse>(createOrIncrementRequest, cancellationToken);
    }

    public Task UpdateAsync(string alertId, AlertUpdateRequest alertRequest, CancellationToken cancellationToken)
    {
        return BaseUpdateAsync(alertId, alertRequest, cancellationToken);
    }

    public Task DeleteAsync(string alertId, CancellationToken cancellationToken)
    {
        return BaseDeleteAsync(alertId, cancellationToken);
    }

    protected override List<KeyValuePair<string, StringValues>> AddSpecificQueryParameters(QueryParameters query)
    {
        var specificParams = new List<KeyValuePair<string, StringValues>>();


        if (query is not AlertQueryParameters alertQuery)
        {
            return specificParams;
        }

        if (!string.IsNullOrWhiteSpace(alertQuery.WithAppId))
        {
            specificParams.Add(KeyValuePair.Create(nameof(alertQuery.WithAppId), new StringValues(alertQuery.WithAppId)));
        }

        if (alertQuery.OfSeverity  is not null)
        {
            specificParams.Add(KeyValuePair.Create(nameof(alertQuery.OfSeverity), new StringValues(alertQuery.OfSeverity.ToString())));
        }

        if (alertQuery.OccurredBefore.HasValue)
        {
            specificParams.Add(KeyValuePair.Create(nameof(alertQuery.OccurredBefore), new StringValues(alertQuery.OccurredBefore.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"))));
        }

        if (alertQuery.OccurredAfter.HasValue)
        {
            specificParams.Add(KeyValuePair.Create(nameof(alertQuery.OccurredAfter), new StringValues(alertQuery.OccurredAfter.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"))));
        }

        return specificParams;
    }
}
