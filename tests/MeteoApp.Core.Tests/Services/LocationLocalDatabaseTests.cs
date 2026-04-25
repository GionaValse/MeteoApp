using System.Linq;
using System.Threading.Tasks;
using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using Xunit;

namespace MeteoApp.Core.Tests.Services;

public class LocationLocalDatabaseTests
{
    private LocationLocalDatabase GetInMemoryDatabase()
    {
        return new LocationLocalDatabase(":memory:");
    }

    [Fact]
    public async Task SaveLocation_ShouldInsertDataCorrectly()
    {
        // Arrange
        var db = GetInMemoryDatabase();
        var location = new LocationModel { Id = "1", Name = "Bari", Latitude = 41.1171, Longitude = 16.8719 };

        // Act
        await db.PushUpsertAsync(location);
        var allLocations = await db.GetDataAsync();

        // Assert
        Assert.Single(allLocations);
        Assert.Equal("Bari", allLocations.First().Name);
    }

    [Fact]
    public async Task GetAllLocations_ShouldReturnAllInsertedData()
    {
        // Arrange
        var db = GetInMemoryDatabase();
        await db.PushUpsertAsync(new LocationModel { Id = "1", Name = "Roma" });
        await db.PushUpsertAsync(new LocationModel { Id = "2", Name = "Milano" });
        await db.PushUpsertAsync(new LocationModel { Id = "3", Name = "Palermo" });

        // Act
        var result = await db.GetDataAsync();

        // Assert
        Assert.Equal(3, result.Count());
        Assert.Contains(result, l => l.Name == "Roma");
        Assert.Contains(result, l => l.Name == "Milano");
        Assert.Contains(result, l => l.Name == "Palermo");
    }

    [Fact]
    public async Task DeleteLocation_ShouldRemoveDataCorrectly()
    {
        // Arrange
        var db = GetInMemoryDatabase();
        var locationToKeep = new LocationModel { Id = "1", Name = "Torino" };
        var locationToDelete = new LocationModel { Id = "2", Name = "Venezia" };

        await db.PushUpsertAsync(locationToKeep);
        await db.PushUpsertAsync(locationToDelete);

        // Act
        await db.PushDeleteAsync(locationToDelete);
        var allLocations = await db.GetDataAsync();

        // Assert
        Assert.Single(allLocations);
        Assert.Equal("Torino", allLocations.First().Name);
    }
}