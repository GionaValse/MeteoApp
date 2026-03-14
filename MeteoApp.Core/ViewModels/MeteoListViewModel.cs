using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using System.Collections.ObjectModel;

namespace MeteoApp.Core.ViewModels;

public class MeteoListViewModel : BaseViewModel
{
    private readonly ILocationProvider _locationProvider;
    private readonly ILocalDatabase _db;
    private readonly IWeatherService _weatherService;
    private readonly IAppConfigProvider _config;

    private ObservableCollection<LocationModel> _locations;
    public ObservableCollection<LocationModel> Locations
    {
        get => _locations;
        set
        {
            _locations = value;
            OnPropertyChanged();
        }
    }

    public MeteoListViewModel(
        ILocationProvider locationProvider,
        IWeatherService weatherService,
        ILocalDatabase database,
        IAppConfigProvider config)
    {
        _locationProvider = locationProvider;
        _weatherService = weatherService;
        _db = database;
        _config = config;

        _locations = new ObservableCollection<LocationModel>();
        Locations = new ObservableCollection<LocationModel>();
    }

    public async Task LoadAllLocationsAsync()
    {
        var location = await _locationProvider.GetCurrentLocationAsync();
        if (location != null)
        {
            var apiKey = _config.GetWeatherApiKey();
            location.Name = await _weatherService.GetNameByPostionAsync(location, apiKey);
            Locations.Add(location);
        }
        var data = _db.GetAllLocations();
        data.ForEach(e => Locations.Add(e));
    }


    public async Task InsertLocationAsync(string name)
    {
        var apiKey = _config.GetWeatherApiKey();
        var location = await _weatherService.GetLocationByNameAsync(name, apiKey);

        if (location == null)
            throw new KeyNotFoundException();
        
        _db.SaveLocation(location);
        Locations.Add(location);
    }
}
