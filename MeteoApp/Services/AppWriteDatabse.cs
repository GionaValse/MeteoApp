using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Services;

public class AppWriteDatabse : IRemoteDatabase<LocationModel>
{
    public Task<IEnumerable<LocationModel>> GetUpdatedSinceAsync(DateTime lastSyncDate)
    {
        throw new NotImplementedException();
    }

    public Task InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public Task PushDeleteAsync(LocationModel entity)
    {
        throw new NotImplementedException();
    }

    public Task PushUpsertAsync(LocationModel entity)
    {
        throw new NotImplementedException();
    }
}
