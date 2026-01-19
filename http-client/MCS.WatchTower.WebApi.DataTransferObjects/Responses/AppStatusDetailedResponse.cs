namespace MCS.WatchTower.WebApi.DataTransferObjects.Responses;

public record AppStatusDetailedResponse : AppStatusResponse
{
    public AppResponse App { get; init; }
}
