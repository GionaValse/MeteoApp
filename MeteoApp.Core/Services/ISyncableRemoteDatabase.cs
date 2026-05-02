using MeteoApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public interface ISyncableRemoteDatabase<T> : IDatabase<T> where T : ISyncableEntity 
{
    Task<IEnumerable<T>> GetUpdatedSinceAsync(DateTime lastSyncDate);
}
