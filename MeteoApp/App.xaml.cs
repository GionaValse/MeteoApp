using System.Globalization;

namespace MeteoApp;

public partial class App : Application
{
    public App(AppShell mainPage)
	{
		InitializeComponent();

        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture;

        MainPage = mainPage;
	}
}