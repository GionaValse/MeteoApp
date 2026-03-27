using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using System.Collections.ObjectModel;

namespace MeteoApp.Core.ViewModels;

public class MeteoListViewModel : BaseViewModel
{
    private readonly ILocationProvider _locationProvider;
    private readonly ILocalDatabase _db;
    private readonly IWeatherService _weatherService;

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
        ILocalDatabase database)
    {
        _locationProvider = locationProvider;
        _weatherService = weatherService;
        _db = database;

        Locations = new ObservableCollection<LocationModel>();
    }

    public async Task LoadAllLocationsAsync()
    {
        var tempStack = new List<LocationModel>();

        var currentLoc = await _locationProvider.GetCurrentLocationAsync();
        if (currentLoc != null)
        {
            var name = await _weatherService.GetNameByPostionAsync(currentLoc);
            currentLoc.Name = name ?? "";
            currentLoc.IsNotGpsLocation = false;
            tempStack.Add(currentLoc);
        }

        var data = _db.GetAllLocations();
        if (data != null)
        {
            tempStack.AddRange(data);
        }

        Locations = new ObservableCollection<LocationModel>(tempStack);
    }


    public async Task InsertLocationAsync(string name)
    {
        var location = await _weatherService.GetLocationByNameAsync(name);

        if (location == null)
            throw new KeyNotFoundException();
        
        _db.SaveLocation(location);
        Locations.Add(location);
    }

    public async Task RemoveLocationAsync(LocationModel location)
    {
        if (location == null)
            return;
        _db.DeleteLocation(location);
        Locations.Remove(location);
    }
}
