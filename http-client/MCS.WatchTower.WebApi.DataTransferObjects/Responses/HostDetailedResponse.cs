namespace MCS.WatchTower.WebApi.DataTransferObjects.Responses;

public record HostDetailedResponse : HostResponse
{
    public IList<AppResponse> Apps { get; init; }
}
