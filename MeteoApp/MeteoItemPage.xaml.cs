using MeteoApp.Core.Models;
using MeteoApp.ViewModels;

namespace MeteoApp;

[QueryProperty(nameof(Location), "Location")]
public partial class MeteoItemPage : ContentPage
{
    private LocationModel _location;
    public LocationModel Location
    {
        get => _location;
        set
        {
            _location = value;
            OnPropertyChanged();
        }
    }

    private WeatherModel _weather;
    public WeatherModel Weather
    {
        get => _weather;
        set
        {
            _weather = value;
            OnPropertyChanged();
        }
    }

    private MeteoViewModel _viewModel;

    private ParameterService _parameterService;


    public MeteoItemPage(MeteoViewModel meteoViewModel, ParameterService parameterService)
    {
        InitializeComponent();
        _viewModel = meteoViewModel;
        _parameterService = parameterService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Weather = await _viewModel.GetWeatherAsync(Location);
        var forecast = await _viewModel.GetForecastAsync(Location);
        try
        {
            _parameterService.Temperatures = forecast.Select(f => f.Main.Temp).ToList();
            _parameterService.Labels = forecast.Select(f => UnixTimeStampToDateTime(f.Dt).ToString("HH:mm")).ToList();
        }catch (Exception ex)
        {
            Console.WriteLine($"Error processing forecast data: {ex.Message}");    
        }
        
    }

    private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).DateTime;
    }
}
