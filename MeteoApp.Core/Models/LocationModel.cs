
namespace MeteoApp.Core.Models;

public class LocationModel : ILocation
{
    public int id { get; set; }
    public string name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
