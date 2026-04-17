using MeteoApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public interface IPreferencesService
{
    UserPreferences GetPreferences();
    void SavePreferences(UserPreferences preferences);
}
