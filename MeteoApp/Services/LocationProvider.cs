using MeteoApp.Core.Models;
using MeteoApp.Core.Services;

namespace MeteoApp.Services;

public class LocationProvider : ILocationProvider
{
    public async Task<LocationModel> GetCurrentLocationAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted && status != PermissionStatus.Restricted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted && status != PermissionStatus.Restricted)
                return null;
        }

        var accuracy = status == PermissionStatus.Restricted
            ? GeolocationAccuracy.Low
            : GeolocationAccuracy.Best;

        try
        {
            var request  = new GeolocationRequest(accuracy, TimeSpan.FromMilliseconds(500));
            var location = await Geolocation.GetLocationAsync(request);

            if (location == null)
                return null;

            return new LocationModel
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Name = "GPS Location"
            };
        }
        catch (FeatureNotEnabledException) { return null; }
        catch (PermissionException)        { return null; }
    }
}