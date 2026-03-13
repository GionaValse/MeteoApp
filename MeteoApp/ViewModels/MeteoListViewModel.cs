using System.Collections.ObjectModel;
using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using Microsoft.Extensions.Configuration;

namespace MeteoApp
{
    public class MeteoListViewModel : BaseViewModel
    {
        private ObservableCollection<LocationModel> _locations;

        private readonly ILocationProvider _locationProvider;
        private Database _db;
        private readonly IWeatherService _weatherService;
        private readonly string _apiKey;

        public ObservableCollection<LocationModel> Locations
        {
            get { return _locations; }
            set
            {
                _locations = value;
                OnPropertyChanged();
            }
        }

        private WeatherModel currentWeather;

        public MeteoListViewModel(
            ILocationProvider locationProvider,
            IWeatherService weatherService,
            Database database,
            IConfiguration config)
        {
            _apiKey = config["MeteoApiKey"];
            _locationProvider = locationProvider;
            _weatherService = weatherService;
            _db = database;
            Locations = new ObservableCollection<LocationModel>();
        }

        public async Task LoadAllLocationsAsync()
        {
            var location = await _locationProvider.GetCurrentLocationAsync();
            if (location != null)
                Locations.Add(location);
            var data = _db.GetAllLocations();
            data.ForEach(e => Locations.Add(e));
        }

        public async void InsertLocation(string name)
        {
            var location = await _weatherService.GetLocationByNameAsync(name, _apiKey);

            if (location == null)
            {
                // TODO: feedback to user: no location found
                return;
            }

            _db.SaveLocation(location);
            Locations.Add(location);
        }
    }
}
