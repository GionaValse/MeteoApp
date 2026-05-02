using MeteoApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Services;

public class ThemeService : IThemeService
{
    public void SetTheme(int themeIndex)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Application.Current.UserAppTheme = themeIndex switch
            {
                1 => AppTheme.Light,
                2 => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };
        });
    }
}
