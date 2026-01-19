namespace MCS.WatchTower.WebApi.DataTransferObjects.Responses;

public record ClientDetailedResponse : ClientResponse
{
    public IList<ConnectionResponse> Connections { get; init; }
}
