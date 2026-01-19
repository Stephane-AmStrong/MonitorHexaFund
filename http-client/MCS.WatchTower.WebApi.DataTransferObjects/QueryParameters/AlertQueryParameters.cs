#nullable enable
using MCS.WatchTower.WebApi.DataTransferObjects.Enumerations;

namespace MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;

public record AlertQueryParameters(
    string? WithAppId = null,
    AlertSeverity? OfSeverity = null,
    DateTime? OccurredBefore = null,
    DateTime? OccurredAfter = null,
    string? SearchTerm = null,
    string? OrderBy = null,
    int? Page = null,
    int? PageSize = null
) : Paging.QueryParameters(SearchTerm, OrderBy, Page, PageSize);
