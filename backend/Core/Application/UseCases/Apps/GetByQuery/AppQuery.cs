#nullable enable
using Domain.Entities;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;

namespace Application.UseCases.Apps.GetByQuery;

public record AppQuery : BaseQuery<App>
{
    public AppQuery(AppQueryParameters queryParameters) : base(queryParameters.SearchTerm, queryParameters.OrderBy, queryParameters.Page, queryParameters.PageSize)
    {
        if (!string.IsNullOrWhiteSpace(queryParameters.WithHostName) || !string.IsNullOrWhiteSpace(queryParameters.WithAppName) || !string.IsNullOrWhiteSpace(queryParameters.WithVersion))
        {
            SetFilterExpression
            (
                app => (string.IsNullOrWhiteSpace(queryParameters.WithHostName) || app.HostName == queryParameters.WithHostName) && (string.IsNullOrWhiteSpace(queryParameters.WithAppName) || app.AppName == queryParameters.WithAppName) && (string.IsNullOrWhiteSpace(queryParameters.WithVersion) || app.Version == queryParameters.WithVersion)
            );
        }
    }
}
