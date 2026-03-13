using System.Text.Json;
using System.Linq;
using MeteoApp.Core.Models.Weather;
using Xunit;

namespace MeteoApp.Core.Tests.Models.Weather;

public class WeatherApiResponseTests
{
    [Fact]
    public void WeatherApiResponse_ShouldDeserialize_ComplexJsonStructure()
    {
        // Arrange
        string json = @"{
            ""weather"": [
                {
                    ""id"": 500,
                    ""main"": ""Rain"",
                    ""description"": ""light rain"",
                    ""icon"": ""10d""
                }
            ],
            ""main"": {
                ""temp"": 298.48,
                ""feels_like"": 298.74,
                ""temp_min"": 297.56,
                ""temp_max"": 300.05,
                ""pressure"": 1015,
                ""humidity"": 64
            }
        }";

        // Act
        var result = JsonSerializer.Deserialize<WeatherApiResponse>(json);

        // Assert
        Assert.NotNull(result);

        Assert.NotNull(result.Weather);
        Assert.Single(result.Weather);
        Assert.Equal("light rain", result.Weather.First().Description);

        Assert.NotNull(result.Main);
        Assert.Equal(298.48, result.Main.Temperature);
        Assert.Equal(298.74, result.Main.FeelsLike);
    }
}
