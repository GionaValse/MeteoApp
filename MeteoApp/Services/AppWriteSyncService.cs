using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Services;

public class AppwriteSyncService : ISyncService<LocationModel>
{
    private readonly ISyncableLocalDatabase<LocationModel> _localDatabase;
    private readonly ISyncableRemoteDatabase<LocationModel> _remoteDatabase;

    public AppwriteSyncService(
        ISyncableLocalDatabase<LocationModel> localDatabase, 
        ISyncableRemoteDatabase<LocationModel> remoteDatabase)
    {
        _localDatabase = localDatabase;
        _remoteDatabase = remoteDatabase;
    }

    public async Task DeleteAsync(LocationModel entity)
    {
        await _localDatabase.PushDeleteAsync(entity);

        try
        {
            await _remoteDatabase.PushDeleteAsync(entity);
            await _localDatabase.PushDeleteAsync(entity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MeteoAppSync] Delete: Sync failed, will retry later: {ex.Message}");
        }
    }

    public async Task EnsureUserSessionAsync()
    {
        await _remoteDatabase.InitializeAsync();
    }

    public async Task<IEnumerable<LocationModel>> GetLocalDataAsync()
    {
        return await _localDatabase.GetDataAsync();
    }

    public async Task SynchronizeAsync(ConflictResolutionStrategy strategy = ConflictResolutionStrategy.LatestWins)
    {
        await EnsureUserSessionAsync();

        await (strategy switch
        {
            ConflictResolutionStrategy.LocalWins => SynchronizeLocalWinsAsync(),
            ConflictResolutionStrategy.RemoteWins => SynchronizeRemoteWinsAsync(),
            ConflictResolutionStrategy.LatestWins => SynchronizeLatestWinsAsync(),
            _ => throw new NotImplementedException(),
        });
    }

    public async Task UpsertAsync(LocationModel entity)
    {
        entity.NeedsSync = true;
        entity.UpdatedAt = DateTime.UtcNow;

        await _localDatabase.PushUpsertAsync(entity);

        try
        {
            await _remoteDatabase.PushUpsertAsync(entity);

            entity.NeedsSync = false;
            await _localDatabase.PushUpsertAsync(entity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MeteoAppSync] Upsert: Sync failed, will retry later: {ex.Message}");
        }
    }

    private async Task SynchronizeLocalWinsAsync()
    {
        Console.WriteLine("[MeteoAppSync] LocalWins sync...");
        var pendingUploads = await _localDatabase.GetRecordsNeedingSyncAsync();

        foreach (var item in pendingUploads)
        {
            try
            {
                if (item.IsDeleted)
                {
                    await _remoteDatabase.PushDeleteAsync(item);
                    await _localDatabase.PushDeleteAsync(item);
                }
                else
                {
                    await _remoteDatabase.PushUpsertAsync(item);
                    item.NeedsSync = false;
                    await _localDatabase.PushUpsertAsync(item);
                }
            }
            catch
            {
                continue;
            }
        }

        DateTime safeDefaultDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime lastSyncDate = Preferences.Default.Get("App_LastSync", safeDefaultDate);

        try
        {
            var cloudChanges = await _remoteDatabase.GetUpdatedSinceAsync(lastSyncDate);
            var localData = await _localDatabase.GetDataAsync();

            var pendingAfterUpload = await _localDatabase.GetRecordsNeedingSyncAsync();

            foreach (var cloudItem in cloudChanges)
            {
                var localItem = localData.FirstOrDefault(l => l.Id == cloudItem.Id);
                bool hasLocalUnsyncedChanges = pendingAfterUpload.Any(l => l.Id == cloudItem.Id);

                if (localItem == null || !hasLocalUnsyncedChanges)
                {
                    cloudItem.NeedsSync = false;
                    if (cloudItem.IsDeleted)
                    {
                        await _localDatabase.PushDeleteAsync(cloudItem);
                    }
                    else
                    {
                        await _localDatabase.PushUpsertAsync(cloudItem);
                    }
                }
            }

            Preferences.Default.Set("App_LastSync", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MeteoAppSync] LocalWins: Sync download failed: {ex.Message}");
        }
    }

    private async Task SynchronizeRemoteWinsAsync()
    {
        Console.WriteLine("[MeteoAppSync] RemoteWins sync...");
        DateTime lastSyncDate = Preferences.Default.Get("App_LastSync", DateTime.MinValue);

        try
        {
            var cloudChanges = await _remoteDatabase.GetUpdatedSinceAsync(lastSyncDate);

            foreach (var cloudItem in cloudChanges)
            {
                cloudItem.NeedsSync = false;

                if (cloudItem.IsDeleted)
                {
                    await _localDatabase.PushDeleteAsync(cloudItem);
                }
                else
                {
                    await _localDatabase.PushUpsertAsync(cloudItem);
                }
            }

            Preferences.Default.Set("App_LastSync", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MeteoAppSync] RemoteWins: Sync download fail: {ex.Message}");
        }

        var pendingUploads = await _localDatabase.GetRecordsNeedingSyncAsync();

        foreach (var item in pendingUploads)
        {
            try
            {
                if (item.IsDeleted)
                {
                    await _remoteDatabase.PushDeleteAsync(item);
                    await _localDatabase.PushDeleteAsync(item);
                }
                else
                {
                    await _remoteDatabase.PushUpsertAsync(item);
                    item.NeedsSync = false;
                    await _localDatabase.PushUpsertAsync(item);
                }
            }
            catch
            {
                continue;
            }
        }
    }

    private async Task SynchronizeLatestWinsAsync()
    {
        Console.WriteLine("[MeteoAppSync] LatestWins sync...");
        var pendingUploads = await _localDatabase.GetRecordsNeedingSyncAsync();

        foreach (var item in pendingUploads)
        {
            try
            {
                if (item.IsDeleted)
                {
                    Console.WriteLine($"[MeteoAppSync] {item.Id} deleted local");

                    await _remoteDatabase.PushDeleteAsync(item);
                    await _localDatabase.PushDeleteAsync(item);
                }
                else
                {
                    Console.WriteLine($"[MeteoAppSync] {item.Id} added local");

                    await _remoteDatabase.PushUpsertAsync(item);
                    item.NeedsSync = false;
                    await _localDatabase.PushUpsertAsync(item);
                }
            }
            catch
            {
                continue;
            }
        }

        DateTime safeDefaultDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime lastSyncDate = Preferences.Default.Get("App_LastSync", safeDefaultDate);

        try
        {
            var cloudChanges = await _remoteDatabase.GetUpdatedSinceAsync(lastSyncDate);
            var localData = await _localDatabase.GetDataAsync();

            Console.WriteLine($"[MeteoAppSync] {localData.Count()} : {cloudChanges.Count()} (local : cloud), {lastSyncDate}");

            foreach (var cloudItem in cloudChanges)
            {
                var localItem = localData.FirstOrDefault(l => l.Id == cloudItem.Id);

                if (localItem == null || cloudItem.UpdatedAt > localItem.UpdatedAt)
                {
                    cloudItem.NeedsSync = false;
                    if (cloudItem.IsDeleted)
                    {
                        Console.WriteLine($"[MeteoAppSync] {cloudItem.Id} deleted cloud");
                        await _localDatabase.PushDeleteAsync(cloudItem);
                    }
                    else
                    {
                        Console.WriteLine($"[MeteoAppSync] {cloudItem.Id} added cloud");
                        await _localDatabase.PushUpsertAsync(cloudItem);
                    }
                }
            }

            Preferences.Default.Set("App_LastSync", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MeteoAppSync] LatestWins: Sync download fail: {ex.Message}");
        }
    }
}
