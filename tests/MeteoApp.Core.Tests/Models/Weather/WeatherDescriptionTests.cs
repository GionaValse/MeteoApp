using System.Text.Json;
using MeteoApp.Core.Models.Weather;
using Xunit;

namespace MeteoApp.Core.Tests.Models.Weather;

public class WeatherDescriptionTests
{
    [Fact]
    public void WeatherDescription_ShouldDeserializeCorrectly_FromJson()
    {
        // Arrange
        string json = @"{ ""description"": ""pioggia leggera"" }";

        // Act
        var result = JsonSerializer.Deserialize<WeatherDescription>(json);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("pioggia leggera", result.Description);
    }
}
