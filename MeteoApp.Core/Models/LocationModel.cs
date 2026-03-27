
using SQLite;
using System.Text.Json.Serialization;

namespace MeteoApp.Core.Models;

public class LocationModel : ILocation
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    [JsonPropertyName("lon")]
    public double Longitude { get; set; }

    [JsonIgnore]
    public bool IsNotGpsLocation { get; set; }
}
