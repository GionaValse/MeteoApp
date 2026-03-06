using MeteoApp.Core.Models;
using MeteoApp.Core.Services;

namespace MeteoApp.Services;

public class GPSService : ILocationProvider
{

    public async Task<LocationModel> GetCurrentLocationAsync()
    {
        var permission = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (permission != PermissionStatus.Granted)
        {
            return null;
        }

        var locationRequest = new GeolocationRequest(GeolocationAccuracy.Best);
        var location = await Geolocation.GetLocationAsync(locationRequest);

        var locationModel = new LocationModel();
        locationModel.Latitude = location.Latitude;
        locationModel.Longitude = location.Longitude;

        return locationModel;
    }
}
