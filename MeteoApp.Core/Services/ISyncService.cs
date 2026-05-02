using MeteoApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public enum ConflictResolutionStrategy
{
    LocalWins,      
    RemoteWins,     
    LatestWins  
}

public interface ISyncService<T> where T : ISyncableEntity
{
    Task EnsureUserSessionAsync();
    Task SynchronizeAsync(ConflictResolutionStrategy strategy = ConflictResolutionStrategy.LatestWins);
    Task UpsertAsync(T entity);
    Task DeleteAsync(T entity);
    Task<IEnumerable<T>> GetLocalDataAsync();
}
