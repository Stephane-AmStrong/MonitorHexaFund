using System.Text.Json.Serialization;

namespace MCS.WatchTower.WebApi.DataTransferObjects.Enumerations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppStatus
{
    Up,
    Down,
}
