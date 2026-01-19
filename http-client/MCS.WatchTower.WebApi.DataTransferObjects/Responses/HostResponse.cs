namespace MCS.WatchTower.WebApi.DataTransferObjects.Responses;

public record HostResponse : IBaseDto
{
    public string Id { get; init; }
    public string Name { get; init; }
}
