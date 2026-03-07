using MeteoApp.Core.Models;

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

    public MeteoItemPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}