using System.Text.Json;
using MeteoApp.Core.Models;
using Xunit;

namespace MeteoApp.Core.Tests.Models;

public class LocationModelTests
{
    [Fact]
    public void LocationModel_ShouldDeserializeCorrectly_FromJson()
    {
        // Arrange
        string json = @"{
            ""name"": ""Lugano"",
            ""lat"": 46.0037,
            ""lon"": 8.9511
        }";

        // Act
        var result = JsonSerializer.Deserialize<LocationModel>(json);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Id);
        Assert.Equal("Lugano", result.Name);
        Assert.Equal(46.0037, result.Latitude);
        Assert.Equal(8.9511, result.Longitude);
    }

    [Fact]
    public void LocationModel_ShouldImplement_ILocation()
    {
        // Arrange & Act
        var model = new LocationModel { Latitude = 10.0, Longitude = 20.0 };

        // Assert
        Assert.IsAssignableFrom<ILocation>(model);
        Assert.Equal(10.0, ((ILocation)model).Latitude);
        Assert.Equal(20.0, ((ILocation)model).Longitude);
    }
}
