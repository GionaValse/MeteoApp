using MeteoApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public interface IDatabase<T> where T : IDatabaseEntity
{
    Task InitializeAsync();
    Task PushUpsertAsync(T entity);
    Task PushDeleteAsync(T entity);
    Task<IEnumerable<T>> GetDataAsync();
}
