namespace MCS.WatchTower.WebApi.DataTransferObjects.Responses;

public record McsMemberResponse : IBaseDto
{
    public string Id { get; init; }
    public string Gaia { get; init; }
    public string Fullname { get; init; }
}
