using System.Collections.Generic;
using System.Threading.Tasks;
using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using MeteoApp.Core.ViewModels;
using Moq;
using Xunit;

namespace MeteoApp.Core.Tests.ViewModels;

public class MeteoListViewModelTests
{
    [Fact]
    public async Task LoadAllLocationsAsync_ShouldPopulateLocations_FromGpsAndDatabase()
    {
        // 1. ARRANGE
        var mockLocationProvider = new Mock<ILocationProvider>();
        var mockWeatherService = new Mock<IWeatherService>();
        var mockSyncService = new Mock<ISyncService<LocationModel>>();
        var mockPreferenceService = new Mock<IPreferencesService>();

        var currentLocation = new LocationModel { Id = "1", Latitude = 46.0037, Longitude = 8.9511 };
        mockLocationProvider
            .Setup(p => p.GetCurrentLocationAsync())
            .ReturnsAsync(currentLocation);

        mockWeatherService
            .Setup(ws => ws.GetNameByPostionAsync(currentLocation))
            .ReturnsAsync("Lugano");

        var fakePreferences = new UserPreferences { 
            Language = "en",
            SyncStrategy = 0
        };
        mockPreferenceService
            .Setup(pr => pr.GetPreferences())
            .Returns(fakePreferences);

        var savedLocations = new List<LocationModel>
        {
            new LocationModel { Id = "2", Name = "Roma", Latitude = 41.9, Longitude = 12.4 },
            new LocationModel { Id = "3", Name = "Milano", Latitude = 45.4, Longitude = 9.1 }
        };
        mockSyncService
            .Setup(sync => sync.GetLocalDataAsync())
            .ReturnsAsync(savedLocations);

        var viewModel = new MeteoListViewModel(
            mockLocationProvider.Object,
            mockWeatherService.Object,
            mockSyncService.Object,
            mockPreferenceService.Object
        );

        // 2. ACT
        await viewModel.LoadAllLocationsAsync();

        // 3. ASSERT
        Assert.Equal(3, viewModel.Locations.Count);

        Assert.Equal("Lugano", viewModel.Locations[0].Name);
        Assert.Equal("Roma", viewModel.Locations[1].Name);
        Assert.Equal("Milano", viewModel.Locations[2].Name);

        mockLocationProvider.Verify(p => p.GetCurrentLocationAsync(), Times.Once);
        mockWeatherService.Verify(ws => ws.GetNameByPostionAsync(currentLocation), Times.Once);
        mockSyncService.Verify(db => db.GetLocalDataAsync(), Times.Once);
    }

    [Fact]
    public async Task InsertLocationAsync_ShouldSaveAndAddLocation_WhenCityIsFound()
    {
        // 1. ARRANGE
        var mockLocationProvider = new Mock<ILocationProvider>();
        var mockWeatherService = new Mock<IWeatherService>();
        var mockSyncService = new Mock<ISyncService<LocationModel>>();
        var mockPreferenceService = new Mock<IPreferencesService>();

        var expectedLocation = new LocationModel { Id = "napoli", Name = "Napoli", Latitude = 40.8518, Longitude = 14.2681 };

        mockWeatherService
            .Setup(ws => ws.GetLocationByNameAsync("Napoli"))
            .ReturnsAsync(expectedLocation);

        var viewModel = new MeteoListViewModel(
            mockLocationProvider.Object,
            mockWeatherService.Object,
            mockSyncService.Object,
            mockPreferenceService.Object
        );

        // 2. ACT
        await viewModel.InsertLocationAsync("Napoli");

        // 3. ASSERT
        Assert.Single(viewModel.Locations);
        Assert.Equal("Napoli", viewModel.Locations[0].Name);

        mockSyncService.Verify(sync => sync.UpsertAsync(expectedLocation), Times.Once);
    }

    [Fact]
    public async Task InsertLocationAsync_ShouldThrowExceptionAndNotSave_WhenCityIsNotFound()
    {
        // 1. ARRANGE
        var mockLocationProvider = new Mock<ILocationProvider>();
        var mockWeatherService = new Mock<IWeatherService>();
        var mockDb = new Mock<ISyncService<LocationModel>>();
        var mockPreferenceService = new Mock<IPreferencesService>();

        mockWeatherService
            .Setup(ws => ws.GetLocationByNameAsync("Paperopoli"))
            .ReturnsAsync((LocationModel?)null);

        var viewModel = new MeteoListViewModel(
            mockLocationProvider.Object,
            mockWeatherService.Object,
            mockDb.Object,
            mockPreferenceService.Object 
        );

        // 2. ACT & 3. ASSERT
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            viewModel.InsertLocationAsync("Paperopoli"));

        Assert.Empty(viewModel.Locations);
        mockDb.Verify(sync => sync.UpsertAsync(It.IsAny<LocationModel>()), Times.Never);
    }
}