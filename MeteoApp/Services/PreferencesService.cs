using MeteoApp.Core.Models;
using MeteoApp.Core.Services;

namespace MeteoApp.Services;

public class PreferencesService : IPreferencesService
{
    public UserPreferences GetPreferences()
    {
        DateTime safeDefaultDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return new UserPreferences
        {
            Language = Preferences.Default.Get("App_Language", "Italiano"),
            NotificationsEnabled = Preferences.Default.Get("App_Notifications", false),
            LastSync = Preferences.Default.Get("App_LastSync", safeDefaultDate),
            Theme = Preferences.Default.Get("App_Theme", 0),
            SyncStrategy = Preferences.Default.Get("App_SyncStrategy", 0)
        };
    }

    public void SavePreferences(UserPreferences preferences)
    {
        Preferences.Default.Set("App_Language", preferences.Language);
        Preferences.Default.Set("App_Notifications", preferences.NotificationsEnabled);
        Preferences.Default.Set("App_LastSync", preferences.LastSync);
        Preferences.Default.Set("App_Theme", preferences.Theme);
        Preferences.Default.Set("App_SyncStrategy", preferences.SyncStrategy);
    }
}
