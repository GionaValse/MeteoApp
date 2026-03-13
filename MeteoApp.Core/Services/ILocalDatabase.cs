using MeteoApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public interface ILocalDatabase
{
    List<LocationModel> GetAllLocations();
    int SaveLocation(LocationModel location);
    int DeleteLocation(LocationModel location);
}
