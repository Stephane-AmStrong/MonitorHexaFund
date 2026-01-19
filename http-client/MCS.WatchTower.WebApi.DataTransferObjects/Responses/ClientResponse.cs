namespace MCS.WatchTower.WebApi.DataTransferObjects.Responses;

public record ClientResponse : IBaseDto
{
    public string Id { get; init; }
    public string Login { get; init; }
    public string Gaia { get; init; }
}
