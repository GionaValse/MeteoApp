using System.Globalization;

namespace MeteoApp;

public partial class App : Application
{
    public App(AppShell mainPage)
	{
		InitializeComponent();

        var theme = Preferences.Default.Get("App_Theme", 0);
        Application.Current.UserAppTheme = theme switch
        {
            1 => AppTheme.Light,
            2 => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };

        string savedLanguage = Preferences.Default.Get("App_Language", "Italiano");
        string code = savedLanguage switch
        {
            "English" => "en",
            "Deutsch" => "de",
            _ => "it"
        };

        var culture = new CultureInfo(code);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        MeteoApp.Resources.Strings.AppResources.Culture = culture;

        MainPage = mainPage;
	}
}