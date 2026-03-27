using MeteoApp.Core.Models;
using SQLite;

namespace MeteoApp.Core.Services;

public class Database : ILocalDatabase
{
    private static class Constants
    {
        public const string DatabaseFilename = "MeteoQlie.db";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;
    }


    private readonly SQLiteConnection _db;

    public Database()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.DatabaseFilename);
        _db = new SQLiteConnection(dbPath, Constants.Flags);
        _db.CreateTable<LocationModel>();
    }

    public Database(string dbPath)
    {
        _db = new SQLiteConnection(dbPath, Constants.Flags);
        _db.CreateTable<LocationModel>();
    }

    public List<LocationModel> GetAllLocations()
    {
        var locations = _db.Table<LocationModel>().ToList();
        locations.ForEach(l => l.IsNotGpsLocation = true);
        return locations;
    }

    public int SaveLocation(LocationModel location)
    {
        return _db.Insert(location);
    }

    public int DeleteLocation(LocationModel location)
    {
        return  _db.Delete(location);
    }
}
