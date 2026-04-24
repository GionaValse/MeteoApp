using MeteoApp.Core.Models;
using SQLite;

namespace MeteoApp.Core.Services;

public class Database : ILocalDatabase<LocationModel>
{
    private static class Constants
    {
        public const string DatabaseFilename = "MeteoQlie.db";

        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;
    }

    private readonly SQLiteAsyncConnection _db;

    public Database()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.DatabaseFilename);

        _db = new SQLiteAsyncConnection(dbPath, Constants.Flags);
        _db.CreateTableAsync<LocationModel>().Wait();
    }

    public Database(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath, Constants.Flags);
        _db.CreateTableAsync<LocationModel>().Wait();
    }

    public async Task<IEnumerable<LocationModel>> GetDataAsync()
    {
        var locations = await _db.Table<LocationModel>()
                                 .Where(l => !l.IsDeleted)
                                 .ToListAsync();

        locations.ForEach(l => l.IsNotGpsLocation = true);

        return locations;
    }

    public async Task SaveAsync(LocationModel location)
    {
        await _db.InsertOrReplaceAsync(location);
    }

    public async Task DeleteAsync(LocationModel location)
    {
        location.IsDeleted = true;
        location.NeedsSync = true;
        location.UpdatedAt = DateTime.UtcNow;

        await _db.UpdateAsync(location);
    }

    public async Task<IEnumerable<LocationModel>> GetRecordsNeedingSyncAsync()
    {
        return await _db.Table<LocationModel>().Where(l => l.NeedsSync).ToListAsync();
    }
}