using MeteoApp.Core.Models;
using Xunit;

namespace MeteoApp.Core.Tests.Models;

public class WeatherModelTests
{
    [Fact]
    public void WeatherModel_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var description = "Cielo sereno";
        var temp = 25.5;
        var feelsLike = 27.2;

        // Act
        var model = new WeatherModel
        {
            Description = description,
            Temperature = temp,
            FeelsLike = feelsLike
        };

        // Assert
        Assert.Equal(description, model.Description);
        Assert.Equal(temp, model.Temperature);
        Assert.Equal(feelsLike, model.FeelsLike);
    }
}
