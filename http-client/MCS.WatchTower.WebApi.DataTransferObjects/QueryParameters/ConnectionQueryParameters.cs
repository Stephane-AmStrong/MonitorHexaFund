#nullable enable
namespace MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;

public record ConnectionQueryParameters(
    string? WithClientGaia = null,
    string? WithAppId = null,
    DateTime? EstablishedBefore = null,
    DateTime? EstablishedAfter = null,
    DateTime? TerminatedBefore = null,
    DateTime? TerminatedAfter = null,
    string? SearchTerm = null,
    string? OrderBy = null,
    int? Page = null,
    int? PageSize = null
) : Paging.QueryParameters(SearchTerm, OrderBy, Page, PageSize);
