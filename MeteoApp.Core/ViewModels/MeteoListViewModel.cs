using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using System.Collections.ObjectModel;

namespace MeteoApp.Core.ViewModels;

public class MeteoListViewModel : BaseViewModel
{
    private readonly ILocationProvider _locationProvider;
    private readonly IWeatherService _weatherService;
    private readonly ISyncService<LocationModel> _syncService;
    private readonly IPreferencesService _preferencesService;

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
        ISyncService<LocationModel> syncService,
        IPreferencesService preferencesService)
    {
        _locationProvider = locationProvider;
        _weatherService = weatherService;
        _syncService = syncService;
        _preferencesService = preferencesService;

        Locations = new ObservableCollection<LocationModel>();
    }

    public async Task LoadAllLocationsAsync()
    {
        Console.WriteLine($"[MeteoAppSync] LoadAllLocations....");
        try
        {
            Console.WriteLine($"[MeteoAppSync] Sync...");
            var strategyIndex = _preferencesService.GetPreferences().SyncStrategy;
            var strategy = (ConflictResolutionStrategy)strategyIndex;
            await _syncService.SynchronizeAsync(strategy);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MeteoAppSync] Syncronization failed. Offline data only.: {ex.Message}");
        }

        var tempStack = new List<LocationModel>();

        var currentLoc = await _locationProvider.GetCurrentLocationAsync();
        if (currentLoc != null)
        {
            var name = await _weatherService.GetNameByPostionAsync(currentLoc);
            currentLoc.Name = name ?? "";
            currentLoc.IsNotGpsLocation = false;
            tempStack.Add(currentLoc);
        }

        var data = await _syncService.GetLocalDataAsync();
        if (data != null)
            tempStack.AddRange(data);

        Locations = new ObservableCollection<LocationModel>(tempStack);
    }


    public async Task InsertLocationAsync(string name)
    {
        var location = await _weatherService.GetLocationByNameAsync(name);

        if (location == null)
            throw new KeyNotFoundException();
        
        await _syncService.UpsertAsync(location);
        Locations.Add(location);
    }

    public async Task RemoveLocationAsync(LocationModel location)
    {
        if (location == null)
            return;
        await _syncService.DeleteAsync(location);
        Locations.Remove(location);
    }
}
