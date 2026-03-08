using MeteoApp.Core.Models;
using MeteoApp.Core.Models.Weather;
using System.Text.Json;

namespace MeteoApp.Core.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient cllient)
    {
        _httpClient = cllient;
    }

    public async Task<WeatherModel?> GetWeatherAsync(ILocation location, string apiKey)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?lat={location.Latitude}&lon={location.Longitude}&appid={apiKey}&units=metric";

        var json = await _httpClient.GetStringAsync(url);

        var apiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(json);

        if (apiResponse != null) 
            return null;

        return new WeatherModel
        {
            Description = apiResponse.weather.First().description,
            Temperature = apiResponse.main.temp,
            FeelsLike = apiResponse.main.feels_like
        };
    }
}
