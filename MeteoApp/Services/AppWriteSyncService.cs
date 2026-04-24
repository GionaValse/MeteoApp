using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Services;

public class AppwriteSyncService : ISyncService<LocationModel>
{
    private readonly ILocalDatabase<LocationModel> _localDatabase;
    private readonly IRemoteDatabase<LocationModel> _remoteDatabase;

    public AppwriteSyncService(
        ILocalDatabase<LocationModel> localDatabase, 
        IRemoteDatabase<LocationModel> remoteDatabase)
    {
        _localDatabase = localDatabase;
        _remoteDatabase = remoteDatabase;
    }

    public async Task DeleteAsync(LocationModel entity)
    {
        await _localDatabase.DeleteAsync(entity);

        try
        {
            await _remoteDatabase.PushDeleteAsync(entity);
            await _localDatabase.DeleteAsync(entity);
        }
        catch (Exception)
        {
            Console.WriteLine("Sync failed, will retry later.");
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

        await _localDatabase.SaveAsync(entity);

        try
        {
            await _remoteDatabase.PushUpsertAsync(entity);

            entity.NeedsSync = false;
            await _localDatabase.SaveAsync(entity);
        }
        catch (Exception)
        {
            Console.WriteLine("Sync failed, will retry later.");
        }
    }

    private async Task SynchronizeLocalWinsAsync()
    {
        var pendingUploads = await _localDatabase.GetRecordsNeedingSyncAsync();

        foreach (var item in pendingUploads)
        {
            try
            {
                if (item.IsDeleted)
                {
                    await _remoteDatabase.PushDeleteAsync(item);
                    await _localDatabase.DeleteAsync(item);
                }
                else
                {
                    await _remoteDatabase.PushUpsertAsync(item);
                    item.NeedsSync = false;
                    await _localDatabase.SaveAsync(item);
                }
            }
            catch
            {
                continue;
            }
        }

        DateTime lastSyncDate = Preferences.Default.Get("App_LastSync", DateTime.MinValue);

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
                        await _localDatabase.DeleteAsync(cloudItem);
                    }
                    else
                    {
                        await _localDatabase.SaveAsync(cloudItem);
                    }
                }
            }

            Preferences.Default.Set("App_LastSync", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Errore in fase di download sync: {ex.Message}");
        }
    }

    private async Task SynchronizeRemoteWinsAsync()
    {
        DateTime lastSyncDate = Preferences.Default.Get("App_LastSync", DateTime.MinValue);

        try
        {
            var cloudChanges = await _remoteDatabase.GetUpdatedSinceAsync(lastSyncDate);

            foreach (var cloudItem in cloudChanges)
            {
                cloudItem.NeedsSync = false;

                if (cloudItem.IsDeleted)
                {
                    await _localDatabase.DeleteAsync(cloudItem);
                }
                else
                {
                    await _localDatabase.SaveAsync(cloudItem);
                }
            }

            Preferences.Default.Set("App_LastSync", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Errore in fase di download sync: {ex.Message}");
        }

        var pendingUploads = await _localDatabase.GetRecordsNeedingSyncAsync();

        foreach (var item in pendingUploads)
        {
            try
            {
                if (item.IsDeleted)
                {
                    await _remoteDatabase.PushDeleteAsync(item);
                    await _localDatabase.DeleteAsync(item);
                }
                else
                {
                    await _remoteDatabase.PushUpsertAsync(item);
                    item.NeedsSync = false;
                    await _localDatabase.SaveAsync(item);
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
        var pendingUploads = await _localDatabase.GetRecordsNeedingSyncAsync();

        foreach (var item in pendingUploads)
        {
            try
            {
                if (item.IsDeleted)
                {
                    await _remoteDatabase.PushDeleteAsync(item);
                    await _localDatabase.DeleteAsync(item);
                }
                else
                {
                    await _remoteDatabase.PushUpsertAsync(item);
                    item.NeedsSync = false;
                    await _localDatabase.SaveAsync(item);
                }
            }
            catch
            {
                continue;
            }
        }

        DateTime lastSyncDate = Preferences.Default.Get("LastSyncDate", DateTime.MinValue);

        try
        {
            var cloudChanges = await _remoteDatabase.GetUpdatedSinceAsync(lastSyncDate);
            var localData = await _localDatabase.GetDataAsync();

            foreach (var cloudItem in cloudChanges)
            {
                var localItem = localData.FirstOrDefault(l => l.Id == cloudItem.Id);

                if (localItem == null || cloudItem.UpdatedAt > localItem.UpdatedAt)
                {
                    cloudItem.NeedsSync = false;
                    if (cloudItem.IsDeleted)
                    {
                        await _localDatabase.DeleteAsync(cloudItem);
                    }
                    else
                    {
                        await _localDatabase.SaveAsync(cloudItem);
                    }
                }
            }

            Preferences.Default.Set("App_LastSync", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Errore in fase di download sync: {ex.Message}");
        }
    }
}
