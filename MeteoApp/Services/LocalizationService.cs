using MeteoApp.Core.Services;
using MeteoApp.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MeteoApp.Services;

public class LocalizationService : ILocalizationService
{
    public void SetLanguage(string language)
    {
        string languageCode = GetLanguageCode(language);

        var culture = new CultureInfo(languageCode);

        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        AppResources.Culture = culture;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Application.Current.MainPage = new AppShell();
        });
    }

    public string GetLanguageCode(string language)
    {
        return language switch
        {
            "English" => "en",
            "Deutsch" => "de",
            _ => "it"
        };
    }
}
