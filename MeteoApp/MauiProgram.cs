using MeteoApp.Core.Services;
using MeteoApp.Core.ViewModels;
using MeteoApp.Services;
using MeteoApp.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace MeteoApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiMaps()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
        builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
        builder.Logging.AddDebug();
#endif

        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("MeteoApp.appsettings.json");
        if (stream != null)
        {
            builder.Configuration.AddJsonStream(stream);
        }

        // --- REGISTRATION SYSTEM SERVICES (MAUI) ---
        builder.Services.AddSingleton<IAppConfigProvider, MauiConfigProvider>();
        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
        builder.Services.AddSingleton<ILocationProvider, LocationProvider>();
        builder.Services.AddSingleton<INotificationProvider, NotificationProvider>();

        // --- REGISTRATION CORE SERVICES ---
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<ILocalDatabase, Database>();
        builder.Services.AddSingleton<IWeatherService, WeatherService>();

        // --- REGISTRATION VIEWMODELS & PAGES ---
        builder.Services.AddTransient<MeteoListViewModel>();
        builder.Services.AddTransient<MeteoViewModel>();
        builder.Services.AddTransient<MapsViewModel>();

        builder.Services.AddTransient<MeteoListPage>();
        builder.Services.AddTransient<MeteoItemPage>();
        builder.Services.AddTransient<MapPage>();

        // -- REGISTRATION BLAZZOR --
        builder.Services.AddSingleton<ParameterService>();
        builder.Services.AddMauiBlazorWebView();

        return builder.Build();
	}
}

