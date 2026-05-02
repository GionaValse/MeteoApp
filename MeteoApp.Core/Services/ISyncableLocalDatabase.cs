using MeteoApp.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public interface ISyncableLocalDatabase<T> : IDatabase<T> where T : ISyncableEntity
{
    Task<IEnumerable<T>> GetRecordsNeedingSyncAsync();
}
