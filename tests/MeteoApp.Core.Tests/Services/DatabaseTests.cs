using System.Linq;
using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using Xunit;

namespace MeteoApp.Core.Tests.Services;

public class DatabaseTests
{
    private Database GetInMemoryDatabase()
    {
        // ":memory:" SQLite do not crate real files
        return new Database(":memory:");
    }

    [Fact]
    public void SaveLocation_ShouldInsertDataCorrectly()
    {
        // Arrange
        var db = GetInMemoryDatabase();
        var location = new LocationModel { Id = 1, Name = "Bari", Latitude = 41.1171, Longitude = 16.8719 };

        // Act
        int rowsAffected = db.SaveLocation(location);
        var allLocations = db.GetAllLocations();

        // Assert
        Assert.Equal(1, rowsAffected);
        Assert.Single(allLocations);
        Assert.Equal("Bari", allLocations.First().Name);
    }

    [Fact]
    public void GetAllLocations_ShouldReturnAllInsertedData()
    {
        // Arrange
        var db = GetInMemoryDatabase();
        db.SaveLocation(new LocationModel { Name = "Roma" });
        db.SaveLocation(new LocationModel { Name = "Milano" });
        db.SaveLocation(new LocationModel { Name = "Palermo" });

        // Act
        var result = db.GetAllLocations();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(result, l => l.Name == "Roma");
        Assert.Contains(result, l => l.Name == "Milano");
        Assert.Contains(result, l => l.Name == "Palermo");
    }

    [Fact]
    public void DeleteLocation_ShouldRemoveDataCorrectly()
    {
        // Arrange
        var db = GetInMemoryDatabase();
        var locationToKeep = new LocationModel { Id = 1, Name = "Torino" };
        var locationToDelete = new LocationModel { Id = 2, Name = "Venezia" };

        db.SaveLocation(locationToKeep);
        db.SaveLocation(locationToDelete);

        // Act
        int rowsAffected = db.DeleteLocation(locationToDelete);
        var allLocations = db.GetAllLocations();

        // Assert
        Assert.Equal(1, rowsAffected);
        Assert.Single(allLocations);
        Assert.Equal("Torino", allLocations.First().Name);
    }
}
