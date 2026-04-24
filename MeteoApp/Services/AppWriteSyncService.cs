using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Services;

public class AppWriteSyncService : ISyncService<LocationModel>
{
    private readonly ILocalDatabase<LocationModel> _localDatabase;
    private readonly IRemoteDatabase<LocationModel> _remoteDatabase;

    public AppWriteSyncService(
        ILocalDatabase<LocationModel> localDatabase, 
        IRemoteDatabase<LocationModel> remoteDatabase)
    {
        _localDatabase = localDatabase;
        _remoteDatabase = remoteDatabase;
    }

    public async Task DeleteAsync(LocationModel entity)
    {
        await _localDatabase.DeleteAsync(entity);
        await _remoteDatabase.PushDeleteAsync(entity);
    }

    public Task EnsureUserSessionAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<LocationModel>> GetLocalDataAsync()
    {
        return await _localDatabase.GetDataAsync();
    }

    public Task SynchronizeAsync(ConflictResolutionStrategy strategy = ConflictResolutionStrategy.LatestWins)
    {
        return strategy switch
        {
            ConflictResolutionStrategy.LocalWins => SynchronizeLocalWinsAsync(),
            ConflictResolutionStrategy.RemoteWins => SynchronizeRemoteWinsAsync(),
            ConflictResolutionStrategy.LatestWins => SynchronizeLatestWinsAsync(),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task UpsertAsync(LocationModel entity)
    {
        await _localDatabase.SaveAsync(entity);
        await _remoteDatabase.PushUpsertAsync(entity);
    }

    private Task SynchronizeLocalWinsAsync()
    {
        throw new NotImplementedException();
    }

    private Task SynchronizeRemoteWinsAsync()
    {
        throw new NotImplementedException();
    }

    private Task SynchronizeLatestWinsAsync()
    {
        throw new NotImplementedException();
    }
}
