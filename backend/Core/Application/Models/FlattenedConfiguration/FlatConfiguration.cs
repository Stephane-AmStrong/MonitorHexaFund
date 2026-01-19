using System.Text.Json.Serialization;

namespace Application.Models.FlattenedConfiguration;

public class FlatConfiguration
{
    [JsonPropertyName("Apps")]
    public List<AppConfig> AppConfigs { get; set; }

    [JsonPropertyName("GenerationInfo")]
    public GenerationInfo GenerationInfo { get; set; }
}
