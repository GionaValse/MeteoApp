using MeteoApp.Core.Models;
using MeteoApp.Core.Services;

namespace MeteoApp.Services;

public class PreferencesService : IPreferencesService
{
    public UserPreferences GetPreferences()
    {
        return new UserPreferences
        {
            Language = Preferences.Default.Get("App_Language", "Italiano"),
            NotificationsEnabled = Preferences.Default.Get("App_Notifications", false),

            LastSync = new DateTime(Preferences.Default.Get("App_LastSync", DateTime.MinValue.Ticks))
        };
    }

    public void SavePreferences(UserPreferences preferences)
    {
        Preferences.Default.Set("App_Language", preferences.Language);
        Preferences.Default.Set("App_Notifications", preferences.NotificationsEnabled);
        Preferences.Default.Set("App_LastSync", preferences.LastSync.Ticks);
    }
}
