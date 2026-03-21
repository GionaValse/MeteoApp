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
        var mockDb = new Mock<ILocalDatabase>();

        float lat = 41.9028f;
        float lon = 12.4964f;

        // Simuliamo che le coordinate corrispondano a Roma
        var expectedLocation = new LocationModel { Name = "Roma", Latitude = lat, Longitude = lon };
        mockWeatherService
            .Setup(ws => ws.GetLocationByLatLonAsync(lat, lon))
            .ReturnsAsync(expectedLocation);

        var viewModel = new MapsViewModel(mockWeatherService.Object, mockDb.Object);

        // 2. ACT
        await viewModel.SaveLocationAsync(lat, lon);

        // 3. ASSERT
        // Verifichiamo che il metodo SaveLocation sia stato chiamato col modello giusto
        mockDb.Verify(db => db.SaveLocation(expectedLocation), Times.Once);
    }

    [Fact]
    public async Task SaveLocationAsync_ShouldNotSave_WhenLocationIsNull()
    {
        // 1. ARRANGE
        var mockWeatherService = new Mock<IWeatherService>();
        var mockDb = new Mock<ILocalDatabase>();

        float lat = 0.0f;
        float lon = 0.0f;

        // Simuliamo coordinate in mezzo all'oceano (nessuna città trovata = null)
        mockWeatherService
            .Setup(ws => ws.GetLocationByLatLonAsync(lat, lon))
            .ReturnsAsync((LocationModel?)null);

        var viewModel = new MapsViewModel(mockWeatherService.Object, mockDb.Object);

        // 2. ACT
        // Anche se il codice lancia KeyNotFoundException, viene catturata dal catch,
        // quindi il metodo non deve crashare durante il test.
        await viewModel.SaveLocationAsync(lat, lon);

        // 3. ASSERT
        // Il database NON deve essere mai toccato
        mockDb.Verify(db => db.SaveLocation(It.IsAny<LocationModel>()), Times.Never);
    }

    [Fact]
    public async Task SaveLocationAsync_ShouldHandleApiExceptionsGracefully()
    {
        // 1. ARRANGE
        var mockWeatherService = new Mock<IWeatherService>();
        var mockDb = new Mock<ILocalDatabase>();

        // Simuliamo che manchi internet e il servizio esploda lanciando un'eccezione
        mockWeatherService
            .Setup(ws => ws.GetLocationByLatLonAsync(It.IsAny<float>(), It.IsAny<float>()))
            .ThrowsAsync(new Exception("Network error"));

        var viewModel = new MapsViewModel(mockWeatherService.Object, mockDb.Object);

        // 2. ACT
        // Il try-catch nel ViewModel deve assorbire questa eccezione senza far fallire il test
        await viewModel.SaveLocationAsync(10f, 10f);

        // 3. ASSERT
        // Anche in caso di errore di rete, il database è salvo e non viene chiamato
        mockDb.Verify(db => db.SaveLocation(It.IsAny<LocationModel>()), Times.Never);
    }
}
