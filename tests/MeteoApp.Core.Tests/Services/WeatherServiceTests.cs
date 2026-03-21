using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using Moq;
using Moq.Protected;
using Xunit;

namespace MeteoApp.Core.Tests.Services;

public class WeatherServiceTests
{
    private HttpClient CreateMockedHttpClient(string responseJson, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(responseJson)
            });

        return new HttpClient(handlerMock.Object);
    }

    // --- TEST ECCEZIONE CHIAVE API (Ne basta uno per tutti i metodi!) ---

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task SendRequestAsync_ShouldThrowException_WhenApiKeyIsInvalid(string invalidApiKey)
    {
        // Arrange
        var httpClient = new HttpClient();
        var mockConfig = new Mock<IAppConfigProvider>();

        mockConfig.Setup(c => c.GetWeatherApiKey()).Returns(invalidApiKey);

        var service = new WeatherService(httpClient, mockConfig.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GetLocationByNameAsync("Roma"));
    }

    // --- TEST GET LOCATION BY NAME ---

    [Fact]
    public async Task GetLocationByNameAsync_ShouldReturnLocation_WhenApiReturnsData()
    {
        // Arrange
        string jsonResponse = @"[
            { ""name"": ""Milano"", ""lat"": 45.4642, ""lon"": 9.1900 }
        ]";
        var httpClient = CreateMockedHttpClient(jsonResponse);

        var mockConfig = new Mock<IAppConfigProvider>();
        mockConfig.Setup(c => c.GetWeatherApiKey()).Returns("fake-api-key");

        var service = new WeatherService(httpClient, mockConfig.Object);

        // Act
        var result = await service.GetLocationByNameAsync("Milano");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Milano", result.Name);
        Assert.Equal(45.4642, result.Latitude);
        Assert.Equal(9.1900, result.Longitude);
    }

    [Fact]
    public async Task GetLocationByNameAsync_ShouldReturnNull_WhenApiReturnsEmptyArray()
    {
        // Arrange
        string jsonResponse = "[]";
        var httpClient = CreateMockedHttpClient(jsonResponse);

        var mockConfig = new Mock<IAppConfigProvider>();
        mockConfig.Setup(c => c.GetWeatherApiKey()).Returns("fake-api-key");

        var service = new WeatherService(httpClient, mockConfig.Object);

        // Act
        var result = await service.GetLocationByNameAsync("CittàInesistente");

        // Assert
        Assert.Null(result);
    }

    // --- TEST GET LOCATION BY LAT/LON (Nuovo!) ---

    [Fact]
    public async Task GetLocationByLatLonAsync_ShouldReturnLocation_WhenApiReturnsData()
    {
        // Arrange
        string jsonResponse = @"[ { ""name"": ""Lugano"", ""lat"": 46.0, ""lon"": 8.9 } ]";
        var httpClient = CreateMockedHttpClient(jsonResponse);

        var mockConfig = new Mock<IAppConfigProvider>();
        mockConfig.Setup(c => c.GetWeatherApiKey()).Returns("fake-api-key");

        var service = new WeatherService(httpClient, mockConfig.Object);

        // Act
        var result = await service.GetLocationByLatLonAsync(46.0f, 8.9f);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Lugano", result.Name);
    }

    // --- TEST GET WEATHER ---

    [Fact]
    public async Task GetWeatherAsync_ShouldReturnWeatherModel_WhenApiReturnsValidData()
    {
        // Arrange
        string jsonResponse = @"{
            ""weather"": [{ ""description"": ""soleggiato"" }],
            ""main"": { ""temp"": 22.5, ""feels_like"": 24.0 }
        }";
        var httpClient = CreateMockedHttpClient(jsonResponse);

        var mockConfig = new Mock<IAppConfigProvider>();
        mockConfig.Setup(c => c.GetWeatherApiKey()).Returns("fake-api-key");

        var service = new WeatherService(httpClient, mockConfig.Object);
        var fakeLocation = new LocationModel { Latitude = 45.0, Longitude = 9.0 };

        // Act
        var result = await service.GetWeatherAsync(fakeLocation);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("soleggiato", result.Description);
        Assert.Equal(22.5, result.Temperature);
    }

    // --- TEST GET NAME BY POSITION (Nuovo!) ---

    [Fact]
    public async Task GetNameByPostionAsync_ShouldReturnStringName_WhenApiReturnsData()
    {
        // Arrange
        string jsonResponse = @"[ { ""name"": ""Chiasso"" } ]";
        var httpClient = CreateMockedHttpClient(jsonResponse);

        var mockConfig = new Mock<IAppConfigProvider>();
        mockConfig.Setup(c => c.GetWeatherApiKey()).Returns("fake-api-key");

        var service = new WeatherService(httpClient, mockConfig.Object);
        var fakeLocation = new LocationModel { Latitude = 45.8, Longitude = 9.0 };

        // Act
        var result = await service.GetNameByPostionAsync(fakeLocation);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Chiasso", result);
    }
}
