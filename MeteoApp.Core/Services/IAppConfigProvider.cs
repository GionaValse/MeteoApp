using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public interface IAppConfigProvider
{
    string GetWeatherApiKey();
    string GetAppwriteEndpoint();
    string GetAppwriteProjectId();
    string GetAppwriteApiKey();
}
