namespace MCS.WatchTower.WebApi.DataTransferObjects.Responses;

public record AlertDetailedResponse : AlertResponse
{
    public AppResponse App { get; init; }
}
