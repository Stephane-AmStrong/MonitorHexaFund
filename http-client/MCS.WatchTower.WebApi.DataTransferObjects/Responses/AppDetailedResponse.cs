namespace MCS.WatchTower.WebApi.DataTransferObjects.Responses;

public record AppDetailedResponse : AppResponse
{
    public IList<ConnectionResponse> Connections { get; init; }
    public IList<AppStatusResponse> Statuses { get; init; }
}
