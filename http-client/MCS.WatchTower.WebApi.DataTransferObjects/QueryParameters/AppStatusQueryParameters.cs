#nullable enable
using MCS.WatchTower.WebApi.DataTransferObjects.Enumerations;

namespace MCS.WatchTower.WebApi.DataTransferObjects.QueryParameters;

public record AppStatusQueryParameters(
    string? WithAppId = null,
    AppStatus? OfStatus = null,
    DateTime? RecordedBefore = null,
    DateTime? RecordedAfter = null,
    string? SearchTerm = null,
    string? OrderBy = null,
    int? Page = null,
    int? PageSize = null
) : Paging.QueryParameters(SearchTerm, OrderBy, Page, PageSize);
