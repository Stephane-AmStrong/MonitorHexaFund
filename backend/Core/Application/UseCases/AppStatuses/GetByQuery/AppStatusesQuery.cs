#nullable enable
using Domain.Entities;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;

namespace Application.UseCases.AppStatuses.GetByQuery;

public record AppStatusQuery : BaseQuery<AppStatus>
{
    public AppStatusQuery(AppStatusQueryParameters queryParameters) : base(queryParameters.SearchTerm, queryParameters.OrderBy, queryParameters.Page, queryParameters.PageSize)
    {
        if (!string.IsNullOrWhiteSpace(queryParameters.WithAppId) || queryParameters.OfStatus is not null || queryParameters.RecordedBefore is not null || queryParameters.RecordedAfter is not null)
        {
            SetFilterExpression
            (
                appStatus => (string.IsNullOrWhiteSpace(queryParameters.WithAppId) || appStatus.AppId == queryParameters.WithAppId) &&
                        (queryParameters.OfStatus == null || appStatus.Status == queryParameters.OfStatus.ToString()) &&
                        (queryParameters.RecordedBefore == null || appStatus.RecordedAt < queryParameters.RecordedBefore) &&
                        (queryParameters.RecordedAfter == null || appStatus.RecordedAt >= queryParameters.RecordedAfter)
            );
        }
    }
}