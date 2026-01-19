namespace MCS.WatchTower.WebApi.DataTransferObjects.Requests;

public record McsMemberUpdateRequest
{
    public string Gaia { get; init; }
    public string Fullname { get; init; }
    public HashSet<string> Machines { get; init; } = [];
}
