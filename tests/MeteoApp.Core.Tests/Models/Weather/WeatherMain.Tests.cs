using System.Text.Json;
using MeteoApp.Core.Models.Weather;
using Xunit;

namespace MeteoApp.Core.Tests.Models.Weather;

public class WeatherMainTests
{
    [Fact]
    public void WeatherMain_ShouldDeserializeCorrectly_FromJson()
    {
        // Arrange
        string json = @"{ ""temp"": 18.5, ""feels_like"": 17.2 }";

        // Act
        var result = JsonSerializer.Deserialize<WeatherMain>(json);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(18.5, result.Temperature);
        Assert.Equal(17.2, result.FeelsLike);
    }
}
