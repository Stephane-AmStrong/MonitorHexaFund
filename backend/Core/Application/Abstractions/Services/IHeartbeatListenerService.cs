using MCS.WatchTower.WebApi.DataTransferObjects.Enumerations;

namespace Application.Abstractions.Services;

public interface IHeartbeatListenerService
{
    void OnAppStatusReceived(string appId, AppStatus status);
}
