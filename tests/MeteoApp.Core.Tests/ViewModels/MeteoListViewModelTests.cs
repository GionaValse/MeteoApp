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
        var mockLocationProvider = new Mock<ILocationProvider>();
        var mockWeatherService = new Mock<IWeatherService>();
        var mockDb = new Mock<ILocalDatabase>();
        var mockConfig = new Mock<IAppConfigProvider>();

        var currentLocation = new LocationModel { Id = 1, Name = "Lugano", Latitude = 46.0037, Longitude = 8.9511 };
        mockLocationProvider
            .Setup(p => p.GetCurrentLocationAsync())
            .ReturnsAsync(currentLocation);

        var savedLocations = new List<LocationModel>
        {
            new LocationModel { Id = 2, Name = "Roma", Latitude = 41.9, Longitude = 12.4 },
            new LocationModel { Id = 3, Name = "Milano", Latitude = 45.4, Longitude = 9.1 }
        };
        mockDb
            .Setup(db => db.GetAllLocations())
            .Returns(savedLocations);

        var viewModel = new MeteoListViewModel(
            mockLocationProvider.Object,
            mockWeatherService.Object,
            mockDb.Object,
            mockConfig.Object
        );

        await viewModel.LoadAllLocationsAsync();

        Assert.Equal(3, viewModel.Locations.Count);

        Assert.Equal("Lugano", viewModel.Locations[0].Name);
        Assert.Equal("Roma", viewModel.Locations[1].Name);
        Assert.Equal("Milano", viewModel.Locations[2].Name);

        mockLocationProvider.Verify(p => p.GetCurrentLocationAsync(), Times.Once);
        mockDb.Verify(db => db.GetAllLocations(), Times.Once);
    }

    [Fact]
    public async Task InsertLocationAsync_ShouldSaveAndAddLocation_WhenCityIsFound()
    {
        // 1. ARRANGE
        var mockLocationProvider = new Mock<ILocationProvider>();
        var mockWeatherService = new Mock<IWeatherService>();
        var mockDb = new Mock<ILocalDatabase>();
        var mockConfig = new Mock<IAppConfigProvider>();

        mockConfig.Setup(c => c.GetWeatherApiKey()).Returns("test-key");

        var expectedLocation = new LocationModel { Name = "Napoli", Latitude = 40.8518, Longitude = 14.2681 };
        mockWeatherService
            .Setup(ws => ws.GetLocationByNameAsync("Napoli", "test-key"))
            .ReturnsAsync(expectedLocation);

        var viewModel = new MeteoListViewModel(
            mockLocationProvider.Object,
            mockWeatherService.Object,
            mockDb.Object,
            mockConfig.Object
        );

        // 2. ACT
        await viewModel.InsertLocationAsync("Napoli");

        // 3. ASSERT
        Assert.Single(viewModel.Locations);
        Assert.Equal("Napoli", viewModel.Locations[0].Name);

        mockDb.Verify(db => db.SaveLocation(expectedLocation), Times.Once);
    }

    [Fact]
    public async Task InsertLocationAsync_ShouldNotSave_WhenCityIsNotFound()
    {
        // 1. ARRANGE
        var mockLocationProvider = new Mock<ILocationProvider>();
        var mockWeatherService = new Mock<IWeatherService>();
        var mockDb = new Mock<ILocalDatabase>();
        var mockConfig = new Mock<IAppConfigProvider>();

        mockConfig.Setup(c => c.GetWeatherApiKey()).Returns("test-key");

        mockWeatherService
            .Setup(ws => ws.GetLocationByNameAsync("Paperopoli", "test-key"))
            .ReturnsAsync((LocationModel?)null);

        var viewModel = new MeteoListViewModel(
            mockLocationProvider.Object,
            mockWeatherService.Object,
            mockDb.Object,
            mockConfig.Object
        );

        // 2. ACT
        await viewModel.InsertLocationAsync("Paperopoli");

        // 3. ASSERT
        Assert.Empty(viewModel.Locations);

        mockDb.Verify(db => db.SaveLocation(It.IsAny<LocationModel>()), Times.Never);
    }
}
