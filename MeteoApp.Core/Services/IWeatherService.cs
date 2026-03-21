using MeteoApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public interface IWeatherService
{
    Task<LocationModel?> GetLocationByNameAsync(string cityName);
    Task<LocationModel?> GetLocationByLatLonAsync(float lat, float lon);
    Task<WeatherModel?> GetWeatherAsync(ILocation location);
    Task<string?> GetNameByPostionAsync(ILocation location);
}
