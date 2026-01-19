namespace MCS.WatchTower.WebApi.DataTransferObjects.Responses;

public record McsMemberDetailedResponse : McsMemberResponse
{
    public HashSet<string> Machines { get; init; } = [];
}
