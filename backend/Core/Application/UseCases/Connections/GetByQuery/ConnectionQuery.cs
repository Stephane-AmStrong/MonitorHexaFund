#nullable enable
using Domain.Entities;
using Domain.Shared.Common;
using MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;

namespace Application.UseCases.Connections.GetByQuery;

public record class ConnectionQuery : BaseQuery<Connection>
{
    public ConnectionQuery(ConnectionQueryParameters queryParameters) : base(queryParameters.SearchTerm, queryParameters.OrderBy, queryParameters.Page, queryParameters.PageSize)
    {
        if (!string.IsNullOrWhiteSpace(queryParameters.WithClientGaia) ||
            !string.IsNullOrWhiteSpace(queryParameters.WithAppId) ||
            queryParameters.EstablishedBefore is not null ||
            queryParameters.EstablishedAt is not null ||
            queryParameters.EstablishedAfter is not null ||
            queryParameters.TerminatedBefore is not null ||
            queryParameters.TerminatedAt is not null ||
            queryParameters.TerminatedAfter is not null)
        {
            SetFilterExpression
            (
                connection => (string.IsNullOrWhiteSpace(queryParameters.WithClientGaia) || connection.ClientGaia == queryParameters.WithClientGaia) &&
                              (string.IsNullOrWhiteSpace(queryParameters.WithAppId) || connection.AppId == queryParameters.WithAppId) &&
                              (queryParameters.EstablishedBefore == null || connection.EstablishedAt <= queryParameters.EstablishedBefore) &&
                              (queryParameters.EstablishedAt == null || connection.EstablishedAt == queryParameters.EstablishedAt) &&
                              (queryParameters.EstablishedAfter == null || connection.EstablishedAt >= queryParameters.EstablishedAfter) &&
                              (queryParameters.TerminatedBefore == null || connection.TerminatedAt <= queryParameters.TerminatedBefore) &&
                              (queryParameters.TerminatedAt == null || connection.TerminatedAt == queryParameters.TerminatedAt) &&
                              (queryParameters.TerminatedAfter == null || connection.TerminatedAt >= queryParameters.TerminatedAfter)
            );
        }
    }
}