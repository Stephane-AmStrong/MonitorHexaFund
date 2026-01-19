namespace MCS.WatchTower.WebApi.DataTransferObjects.Responses;

public record ConnectionDetailedResponse : ConnectionResponse
{
    public ClientResponse Client { get; init; }
    public AppResponse App { get; init; }
}
