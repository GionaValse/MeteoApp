using System.Collections.ObjectModel;
using MeteoApp.Core.Models;
using MeteoApp.Core.Services;

namespace MeteoApp
{
    public class MeteoListViewModel : BaseViewModel
    {
        private ObservableCollection<LocationModel> _locations;

        private readonly ILocationProvider _locationProvider;
        private Database _db;

        public ObservableCollection<LocationModel> Locations
        {
            get { return _locations; }
            set
            {
                _locations = value;
                OnPropertyChanged();
            }
        }

        public MeteoListViewModel(ILocationProvider locationProvider)
        {
            _locationProvider = locationProvider;
            _db = new Database();
            Locations = new ObservableCollection<LocationModel>(db.GetAllLocations());
        }

        public async Task LoadCurrentLocationAsync()
        {
            var location = await _locationProvider.GetCurrentLocationAsync();
            if (location != null)
            {
                _db.SaveLocation(location); // salva anche la posizione GPS nel db
                Locations.Add(location);
            }
        }

        public void InsertLocation(string name)
        {
            LocationModel location = new LocationModel();
            location.Name = name;
            _db.SaveLocation(location);
            Locations.Add(location);
        }
    }
}
