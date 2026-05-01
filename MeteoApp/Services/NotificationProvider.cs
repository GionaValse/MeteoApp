using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Services;

public class NotificationProvider : INotificationProvider
{
    private readonly IDatabase<TokenModel> _tokenDatabase;

    public NotificationProvider(IDatabase<TokenModel> tokenDatabase)
    {
        _tokenDatabase = tokenDatabase;
    }

    public async Task<string> RequestTokenAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
        if (status != PermissionStatus.Granted && status != PermissionStatus.Restricted)
        {
            status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted && status != PermissionStatus.Restricted)
                return "";
        }

        var token = await GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            await _tokenDatabase.InitializeAsync();
            await _tokenDatabase.PushUpsertAsync(new TokenModel { Token = token });
        }

        return token;
    }

    private async Task<string> GetTokenAsync()
    {
        var token = "";
#if ANDROID
        await Plugin.Firebase.CloudMessaging.CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
        token = await Plugin.Firebase.CloudMessaging.CrossFirebaseCloudMessaging.Current.GetTokenAsync();
        System.Diagnostics.Debug.WriteLine("Token generated: " + token);
        Console.WriteLine("Token generated: " + token);
#endif
        return token;
    }
}
