using MeteoApp.Core.Models;
using MeteoApp.Services;
using MeteoApp.ViewModels;
using System.Diagnostics;

namespace MeteoApp;

public partial class MeteoListPage : Shell
{
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

    private MeteoListViewModel _listViewModel;

    public MeteoListPage()
    {
         InitializeComponent();
        RegisterRoutes();
        _listViewModel = new MeteoListViewModel(new LocationProvider());
        BindingContext = _listViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _listViewModel.LoadCurrentLocationAsync();
    }

    private void RegisterRoutes()
    {
        Routes.Add("entrydetails", typeof(MeteoItemPage));

        foreach (var item in Routes)
            Routing.RegisterRoute(item.Key, item.Value);
    }

    private async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            LocationModel location = e.SelectedItem as LocationModel;
            var navigationParameter = new Dictionary<string, object>
            {
                { "Location", location }
            };
            await Shell.Current.GoToAsync("entrydetails", navigationParameter);
        }
    }

    private async void OnItemAdded(object sender, EventArgs e)
    {
        string cityname = await DisplayPromptAsync("Aggiungi città", "Inserisci il nome:");
        
        if (!string.IsNullOrEmpty(cityname))
        {
            ((MeteoListViewModel)BindingContext).InsertLocation(cityname);
        }
    }
}
