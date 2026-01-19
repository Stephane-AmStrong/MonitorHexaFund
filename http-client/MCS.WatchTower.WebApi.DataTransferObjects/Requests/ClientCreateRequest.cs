namespace MCS.WatchTower.WebApi.DataTransferObjects.Requests;

public record ClientCreateRequest
{
    public string Gaia { get; init; }
    public string Login { get; init; }
}
