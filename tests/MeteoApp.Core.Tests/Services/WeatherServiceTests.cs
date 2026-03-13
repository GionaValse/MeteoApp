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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetLocationByNameAsync_ShouldThrowException_WhenApiKeyIsInvalid(string invalidApiKey)
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new WeatherService(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GetLocationByNameAsync("Roma", invalidApiKey));
    }

    [Fact]
    public async Task GetLocationByNameAsync_ShouldReturnLocation_WhenApiReturnsData()
    {
        // Arrange
        string jsonResponse = @"[
            { ""name"": ""Milano"", ""lat"": 45.4642, ""lon"": 9.1900 }
        ]";
        var httpClient = CreateMockedHttpClient(jsonResponse);
        var service = new WeatherService(httpClient);

        // Act
        var result = await service.GetLocationByNameAsync("Milano", "fake-api-key");

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
        var service = new WeatherService(httpClient);

        // Act
        var result = await service.GetLocationByNameAsync("CittàInesistente", "fake-api-key");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetWeatherAsync_ShouldThrowException_WhenApiKeyIsInvalid()
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new WeatherService(httpClient);
        var fakeLocation = new LocationModel { Latitude = 10, Longitude = 10 };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GetWeatherAsync(fakeLocation, ""));
    }

    [Fact]
    public async Task GetWeatherAsync_ShouldReturnWeatherModel_WhenApiReturnsValidData()
    {
        // Arrange
        string jsonResponse = @"{
            ""weather"": [{ ""description"": ""soleggiato"" }],
            ""main"": { ""temp"": 22.5, ""feels_like"": 24.0 }
        }";
        var httpClient = CreateMockedHttpClient(jsonResponse);
        var service = new WeatherService(httpClient);
        var fakeLocation = new LocationModel { Latitude = 45.0, Longitude = 9.0 };

        // Act
        var result = await service.GetWeatherAsync(fakeLocation, "fake-api-key");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("soleggiato", result.Description);
        Assert.Equal(22.5, result.Temperature);
        Assert.Equal(24.0, result.FeelsLike);
    }
}
