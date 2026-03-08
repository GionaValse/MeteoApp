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
            _locationProvider = locationProvider;
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

        public void InsertLocation(string name)
        {
            LocationModel location = new LocationModel();
            location.Name = name;
            _db.SaveLocation(location);
            Locations.Add(location);
        }
    }
}
