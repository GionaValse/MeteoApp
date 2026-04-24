using MeteoApp.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public interface ILocalDatabase<T> where T : ISyncableEntity
{
    Task<IEnumerable<T>> GetDataAsync();
    Task SaveAsync(T location);
    Task DeleteAsync(T location);
    Task<IEnumerable<T>> GetRecordsNeedingSyncAsync();
}
