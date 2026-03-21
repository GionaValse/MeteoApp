using MeteoApp.Core.Services;

namespace MeteoApp.Core.ViewModels;

public class MapsViewModel : BaseViewModel
{
    private IWeatherService _weatherService;
    private ILocalDatabase _database;

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
        ILocalDatabase database)
    {
        _weatherService = weatherService;
        _database = database;

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

            _database.SaveLocation(location);
        } 
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
