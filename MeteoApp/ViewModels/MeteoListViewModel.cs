using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using System.Collections.ObjectModel;

namespace MeteoApp
{
    public class MeteoListViewModel : BaseViewModel
    {
        private readonly ILocationProvider _locationProvider;

        public ObservableCollection<LocationModel> Entries { get; } = new();

        public MeteoListViewModel(ILocationProvider locationProvider)
        {
            _locationProvider = locationProvider;
        }

        public async Task LoadCurrentLocationAsync()
        {
            var location = await _locationProvider.GetCurrentLocationAsync();
            if (location != null)
                Entries.Add(location);
        }
    }
}
