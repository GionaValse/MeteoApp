using SQLite;
using System.Text.Json.Serialization;

namespace MeteoApp.Core.Models;

public class LocationModel : ILocation, ISyncableEntity
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    [JsonPropertyName("lon")]
    public double Longitude { get; set; }

    [JsonIgnore]
    public bool IsNotGpsLocation { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;

    public bool NeedsSync { get; set; } = true;
}