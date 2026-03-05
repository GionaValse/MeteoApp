using MeteoApp.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MeteoApp.Core.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient cllient)
    {
        _httpClient = cllient;
    }

    public async Task<WeatherModel> GetWeatherAsync(ILocation location, string apiKey)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?lat={location.Latitude}&lon={location.Longitude}&appid={apiKey}&units=metric";

        var json = await _httpClient.GetStringAsync(url);

        using var doc = JsonDocument.Parse(json);

        return new WeatherModel
        {
            Description = doc.RootElement.GetProperty("weather")[0].GetProperty("description").GetString(),
            Temperature = doc.RootElement.GetProperty("main").GetProperty("temp").GetDouble(),
            FeelsLike = doc.RootElement.GetProperty("main").GetProperty("feels_like").GetDouble()
        };
    }
}
