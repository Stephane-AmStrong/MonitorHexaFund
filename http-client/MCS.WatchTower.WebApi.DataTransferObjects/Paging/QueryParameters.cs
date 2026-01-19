#nullable enable
namespace MCS.WatchTower.WebApi.DataTransferObjects.Paging;

public record QueryParameters(string? SearchTerm, string? OrderBy, int? Page, int? PageSize);
