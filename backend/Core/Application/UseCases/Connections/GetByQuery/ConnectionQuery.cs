#nullable enable
using Domain.Entities;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;

namespace Application.UseCases.Connections.GetByQuery;

public record class ConnectionQuery : BaseQuery<Connection>
{
    public ConnectionQuery(ConnectionQueryParameters queryParameters) : base(queryParameters.SearchTerm, queryParameters.OrderBy, queryParameters.Page, queryParameters.PageSize)
    {
        if (!string.IsNullOrWhiteSpace(queryParameters.WithClientGaia) || !string.IsNullOrWhiteSpace(queryParameters.WithAppId))
        {
            SetFilterExpression
            (
                connection => (string.IsNullOrWhiteSpace(queryParameters.WithClientGaia) || connection.ClientGaia == queryParameters.WithClientGaia) &&
                              (string.IsNullOrWhiteSpace(queryParameters.WithAppId) || connection.AppId == queryParameters.WithAppId)
            );
        }
    }
}