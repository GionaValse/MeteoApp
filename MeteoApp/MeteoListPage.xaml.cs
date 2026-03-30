using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using MeteoApp.Core.ViewModels;
using MeteoApp.Resources.Strings;

namespace MeteoApp;

public partial class MeteoListPage : Shell
{
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

    private MeteoListViewModel _listViewModel;
    private INotificationProvider _notificationProvider;

    public MeteoListPage(MeteoListViewModel viewModel, INotificationProvider notificationProvider)
    {
        InitializeComponent();
        RegisterRoutes();

        _listViewModel = viewModel;
        _notificationProvider = notificationProvider;

        BindingContext = _listViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (_notificationProvider != null)
            await _notificationProvider.RequestTokenAsync();

        if (_listViewModel != null)
            await _listViewModel.LoadAllLocationsAsync();            

    }

    private void RegisterRoutes()
    {
        Routes.Add("entrydetails", typeof(MeteoItemPage));
        Routes.Add("maps", typeof(MapPage));

        foreach (var item in Routes)
            Routing.RegisterRoute(item.Key, item.Value);
    }

    private async void OnListItemSelected(object sender, SelectionChangedEventArgs e)
    {
        var location = e.CurrentSelection.FirstOrDefault() as LocationModel;

        if (location != null)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "Location", location }
            };
            await Shell.Current.GoToAsync("entrydetails", navigationParameter);

            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async void OnItemAdded(object sender, EventArgs e)
    {
        string cityname = await DisplayPromptAsync(AppResources.AddCity, AppResources.InsertName);
        
        if (!string.IsNullOrEmpty(cityname))
        {
            try
            {
                await ((MeteoListViewModel)BindingContext).InsertLocationAsync(cityname);            
            }
            catch (KeyNotFoundException ex)
            {
                await DisplayAlertAsync(
                    AppResources.LocationNotFound, 
                    AppResources.TryAnotherName, 
                    AppResources.ok
                );
            }
        }
    }

    private async void GoToMap(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("maps");
    }

    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
    {
        var swipeItem = sender as SwipeItem;
        var location = swipeItem?.BindingContext as LocationModel;

        if (location == null || !location.IsNotGpsLocation)
            return;

        bool answer = await DisplayAlertAsync("Conferma", $"Vuoi eliminare {location.Name}?", "Sì", "No");

        if (answer)
        {
            await _listViewModel.RemoveLocationAsync(location);
        }
    }
}
