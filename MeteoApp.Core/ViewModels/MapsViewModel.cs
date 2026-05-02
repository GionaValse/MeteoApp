using MeteoApp.Core.Models;
using MeteoApp.Core.Services;

namespace MeteoApp.Core.ViewModels;

public class MapsViewModel : BaseViewModel
{
    private IWeatherService _weatherService;
    private ISyncService<LocationModel> _syncService;

    private string _locationName;
    public string LocationName
    {
        get => _locationName;
        set { _locationName = value; OnPropertyChanged(); }
    }

    private string _coordinates;
    public string Coordinates
    {
        get => _coordinates;
        set { _coordinates = value; OnPropertyChanged(); }
    }

    public MapsViewModel(
        IWeatherService weatherService,
        ISyncService<LocationModel> syncService)
    {
        _weatherService = weatherService;
        _syncService = syncService;

        LocationName = "";
        Coordinates = "";
    }

    public async Task SaveLocationAsync(float lat, float lon)
    {
        try
        {
            var location = await _weatherService.GetLocationByLatLonAsync(lat, lon);

            if (location == null)
                throw new KeyNotFoundException();

            await _syncService.UpsertAsync(location);
        } 
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
