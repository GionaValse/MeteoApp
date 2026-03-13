using MeteoApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public interface IWeatherService
{
    Task<LocationModel?> GetLocationByNameAsync(string cityName, string apiKey);
    Task<WeatherModel?> GetWeatherAsync(ILocation location, string apiKey);
}
