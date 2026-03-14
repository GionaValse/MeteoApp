using System.Text.Json.Serialization;
namespace MeteoApp.Core.Models.Weather;

public class GeoResult
{
    [JsonPropertyName("name")]
    public string Name    { get; set; }
    [JsonPropertyName("country")]
    public string Country { get; set; }
}