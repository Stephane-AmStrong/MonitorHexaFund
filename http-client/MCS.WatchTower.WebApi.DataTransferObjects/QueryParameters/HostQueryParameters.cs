#nullable enable

namespace MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;

public record HostQueryParameters(
    string? WithName = null,
    string? SearchTerm = null,
    string? OrderBy = null,
    int? Page = null,
    int? PageSize = null
) : Paging.QueryParameters(SearchTerm, OrderBy, Page, PageSize);
