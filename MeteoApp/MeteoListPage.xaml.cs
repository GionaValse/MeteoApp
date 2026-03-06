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

    private void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            Entry entry = e.SelectedItem as Entry;

            var navigationParameter = new Dictionary<string, object>
            {
                { "Entry", entry }
            };

            Shell.Current.GoToAsync($"entrydetails", navigationParameter);
        }
    }

    private void OnItemAdded(object sender, EventArgs e)
    {
        _ = ShowPrompt();
    }

    private async Task ShowPrompt()
    {
        await DisplayAlertAsync("Add City", "To Be Implemented", "OK");
    }
}