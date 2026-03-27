using MeteoApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Services;

public class NotificationProvider : INotificationProvider
{
    public async Task<string> RequestTokenAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
        if (status != PermissionStatus.Granted && status != PermissionStatus.Restricted)
        {
            status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted && status != PermissionStatus.Restricted)
                return "";
        }

        return await GetTokenAsync();
    }

    private async Task<string> GetTokenAsync()
    {
        var token = "";
#if ANDROID
        await Plugin.Firebase.CloudMessaging.CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
        token = await Plugin.Firebase.CloudMessaging.CrossFirebaseCloudMessaging.Current.GetTokenAsync();
        System.Diagnostics.Debug.WriteLine("Token generated: " + token);
        Console.WriteLine("Token generated: " + token);
        await Clipboard.Default.SetTextAsync(token);
#endif
        return token;
    }
}
