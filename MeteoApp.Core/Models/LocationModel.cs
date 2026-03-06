
namespace MeteoApp.Core.Models;

public class LocationModel : ILocation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
