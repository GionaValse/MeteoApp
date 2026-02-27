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
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
        builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
        builder.Logging.AddDebug();
#endif

        // var assembly = Assembly.GetExecutingAssembly();
        // using var stream = assembly.GetManifestResourceStream("MeteoApp.appsettings.json");
        // if (stream != null)
        // {
        //     builder.Configuration.AddJsonStream(stream);
        // }

        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
        builder.Services.AddSingleton<HttpClient>();
        
		builder.Services.AddTransient<MeteoViewModel>();
		builder.Services.AddTransient<MeteoListPage>();

        return builder.Build();
	}
}

