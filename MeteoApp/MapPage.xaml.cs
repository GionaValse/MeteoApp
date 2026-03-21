using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using MeteoApp.Core.ViewModels;
using MeteoApp.Resources.Strings;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MeteoApp;

public partial class MapPage : ContentPage
{

    private ILocationProvider _locationProvider;
    private MapsViewModel _mapsViewModel;

	public MapPage(MapsViewModel mapsViewModel, ILocationProvider locationProvider)
	{
        _mapsViewModel = mapsViewModel;
        _locationProvider = locationProvider;

		InitializeComponent();

        BindingContext = _mapsViewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LocationModel location = await _locationProvider.GetCurrentLocationAsync();

        if (location != null) {
            MyMap.IsShowingUser = true;

            var currentLocation = new Location(location.Latitude, location.Longitude);
            var currentRegion = MapSpan.FromCenterAndRadius(currentLocation, Distance.FromKilometers(1));
            MyMap.MoveToRegion(currentRegion);

            _mapsViewModel.LocationName = location.Name;
            _mapsViewModel.Coordinates = $"{location.Latitude:F4}, {location.Longitude:F4}";
        }
    }

    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        var lat = e.Location.Latitude;
        var lon = e.Location.Longitude;

        var placemarks = await Geocoding.Default.GetPlacemarksAsync(lat, lon);
        var place = placemarks?.FirstOrDefault();

        _mapsViewModel.LocationName = place.Locality ?? AppResources.UnknownPoint;
        _mapsViewModel.Coordinates = $"{lat:F4}, {lon:F4}";

        MyMap.Pins.Clear();
        MyMap.Pins.Add(new Pin
        {
            Label = _mapsViewModel.LocationName,
            Location = e.Location,
            Type = PinType.Place
        });
    }

    private async void OnAddLocationClicked(object sender, EventArgs e)
    {
        var pin = MyMap.Pins.FirstOrDefault();
        if (pin != null)
        {
            await _mapsViewModel.SaveLocationAsync((float)pin.Location.Latitude, (float)pin.Location.Longitude);
            await DisplayAlertAsync(AppResources.LocalityAdded, AppResources.LocalitySaved, AppResources.ok);

            await Shell.Current.GoToAsync("..");
        }
    }

    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        string query = LocationSearchBar.Text;
        if (string.IsNullOrWhiteSpace(query)) return;

        try
        {
            var locations = await Geocoding.Default.GetLocationsAsync(query);
            var location = locations?.FirstOrDefault();

            if (location != null)
            {
                _mapsViewModel.LocationName = query;
                _mapsViewModel.Coordinates = $"{location.Latitude:F4}, {location.Longitude:F4}";

                var mapSpan = Microsoft.Maui.Maps.MapSpan.FromCenterAndRadius(
                    new Location(location.Latitude, location.Longitude),
                    Microsoft.Maui.Maps.Distance.FromKilometers(5));

                MyMap.MoveToRegion(mapSpan);

                MyMap.Pins.Clear();
                MyMap.Pins.Add(new Microsoft.Maui.Controls.Maps.Pin
                {
                    Label = query,
                    Location = new Location(location.Latitude, location.Longitude),
                    Type = Microsoft.Maui.Controls.Maps.PinType.Place
                });

                LocationSearchBar.Unfocus();
            }
            else
            {
                await DisplayAlertAsync(
                    AppResources.LocationNotFound,
                    AppResources.TryAnotherName,
                    AppResources.ok
                    );
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Errore", "Impossibile effettuare la ricerca: " + ex.Message, AppResources.ok);
        }
    }
}