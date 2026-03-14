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


    public MeteoItemPage(MeteoViewModel meteoViewModel)
    {
        InitializeComponent();
        _viewModel = meteoViewModel;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Weather = await _viewModel.GetWeatherAsync(Location);
    }
}
