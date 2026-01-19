using System.Text.Json.Serialization;

namespace MCS.WatchTower.WebApi.DataTransferObjects.Enumerations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AlertSeverity
{
    Info,
    Low,
    Warning,
    High,
    Critical,
    Emergency
}