namespace MCS.WatchTower.WebApi.DataTransferObjects.Requests;

public record McsMemberCreateRequest : McsMemberUpdateRequest
{
    public string Id { get; init; }
}
