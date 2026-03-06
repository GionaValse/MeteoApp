using MeteoApp.Core.Models;
using MeteoApp.Core.Services;

namespace MeteoApp.Services;

public class LocationProvider : ILocationProvider
{
    public async Task<LocationModel> GetCurrentLocationAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
                return null;
        }

        var location = await Geolocation.GetLocationAsync();

        if (location == null)
            return null;

        return new LocationModel
        {
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Name = "GPS Location"
        };
    }
}
