using MeteoApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public interface IRemoteDatabase<T> where T : ISyncableEntity 
{
    Task InitializeAsync();
    Task<IEnumerable<T>> GetUpdatedSinceAsync(DateTime lastSyncDate);
    Task PushUpsertAsync(T entity);
    Task PushDeleteAsync(T entity);
}
