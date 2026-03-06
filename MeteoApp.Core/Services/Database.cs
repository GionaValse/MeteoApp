using MeteoApp.Core.Models;
using SQLite;

namespace MeteoApp.Core.Services;

public class Database
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


    private readonly SQLiteAsyncConnection _db;

    public Database()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.DatabaseFilename);
        _db = new SQLiteAsyncConnection(dbPath, Constants.Flags);
        _db.CreateTableAsync<LocationModel>();
    }

    public async Task<List<LocationModel>> GetAllLocations()
    {
        return await _db.Table<LocationModel>().ToListAsync();
    }

    public async Task<int> SaveLocation(LocationModel location)
    {
        return await _db.InsertAsync(location);
    }

    public async Task<int> DeleteLocation(LocationModel location)
    {
        return await _db.DeleteAsync(location);
    }
}