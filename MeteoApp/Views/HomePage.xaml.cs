using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using MeteoApp.Core.ViewModels;

namespace MeteoApp.Views;

public partial class HomePage : ContentPage
{
    private MeteoListViewModel _listViewModel;
    private INotificationProvider _notificationProvider;

    public HomePage(MeteoListViewModel viewModel, INotificationProvider notificationProvider)
    {
        InitializeComponent();

        _listViewModel = viewModel;
        _notificationProvider = notificationProvider;

        BindingContext = _listViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _listViewModel.LoadAllLocationsAsync();
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