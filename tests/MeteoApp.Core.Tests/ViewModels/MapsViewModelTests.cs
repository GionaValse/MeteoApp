using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using MeteoApp.Core.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Tests.ViewModels;

public class MapsViewModelTests
{
    [Fact]
    public async Task SaveLocationAsync_ShouldSaveToDatabase_WhenLocationIsFound()
    {
        // 1. ARRANGE
        var mockWeatherService = new Mock<IWeatherService>();
        var mockSyncService = new Mock<ISyncService<LocationModel>>();

        float lat = 41.9028f;
        float lon = 12.4964f;

        var expectedLocation = new LocationModel { Id = "roma", Name = "Roma", Latitude = lat, Longitude = lon };
        mockWeatherService
            .Setup(ws => ws.GetLocationByLatLonAsync(lat, lon))
            .ReturnsAsync(expectedLocation);

        var viewModel = new MapsViewModel(mockWeatherService.Object, mockSyncService.Object);

        // 2. ACT
        await viewModel.SaveLocationAsync(lat, lon);

        // 3. ASSERT
        mockSyncService.Verify(sync => sync.UpsertAsync(expectedLocation), Times.Once);
    }

    [Fact]
    public async Task SaveLocationAsync_ShouldNotSave_WhenLocationIsNull()
    {
        // 1. ARRANGE
        var mockWeatherService = new Mock<IWeatherService>();
        var mockSyncService = new Mock<ISyncService<LocationModel>>();

        float lat = 0.0f;
        float lon = 0.0f;

        mockWeatherService
            .Setup(ws => ws.GetLocationByLatLonAsync(lat, lon))
            .ReturnsAsync((LocationModel?)null);

        var viewModel = new MapsViewModel(mockWeatherService.Object, mockSyncService.Object);

        // 2. ACT
        await viewModel.SaveLocationAsync(lat, lon);

        // 3. ASSERT
        mockSyncService.Verify(sync => sync.UpsertAsync(It.IsAny<LocationModel>()), Times.Never);
    }

    [Fact]
    public async Task SaveLocationAsync_ShouldHandleApiExceptionsGracefully()
    {
        // 1. ARRANGE
        var mockWeatherService = new Mock<IWeatherService>();
        var mockDb = new Mock<ISyncService<LocationModel>>();

        mockWeatherService
            .Setup(ws => ws.GetLocationByLatLonAsync(It.IsAny<float>(), It.IsAny<float>()))
            .ThrowsAsync(new Exception("Network error"));

        var viewModel = new MapsViewModel(mockWeatherService.Object, mockDb.Object);

        // 2. ACT
        await viewModel.SaveLocationAsync(10f, 10f);

        // 3. ASSERT
        mockDb.Verify(db => db.UpsertAsync(It.IsAny<LocationModel>()), Times.Never);
    }
}
