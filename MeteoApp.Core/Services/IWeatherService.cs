using MeteoApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public interface IWeatherService
{
    Task<WeatherModel?> GetWeatherAsync(ILocation location, string apiKey);
}
