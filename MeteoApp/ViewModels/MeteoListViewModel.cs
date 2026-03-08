using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;

namespace MeteoApp
{
    public class MeteoListViewModel : BaseViewModel
    {
        private readonly ILocationProvider _locationProvider;
        private readonly IWeatherService _weatherService;
        private readonly string _apiKey;

        public ObservableCollection<LocationModel> Locations { get; } = new();

        private WeatherModel currentWeather;

        public MeteoListViewModel(
            ILocationProvider locationProvider,
        IWeatherService weatherService,
        IConfiguration config)
        {
            _locationProvider = locationProvider;
        }

        public async Task LoadCurrentLocationAsync()
        {
            var location = await _locationProvider.GetCurrentLocationAsync();
            if (location != null)
                Locations.Add(location);
        }
    }
}
