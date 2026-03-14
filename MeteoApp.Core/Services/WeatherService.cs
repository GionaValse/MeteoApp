using MeteoApp.Core.Models;
using MeteoApp.Core.Models.Weather;
using System.Text.Json;

namespace MeteoApp.Core.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;

    public WeatherService(HttpClient cllient)
    {
        _httpClient = cllient;
        _apiUrl = "https://api.openweathermap.org";
    }

    public async Task<LocationModel?> GetLocationByNameAsync(string cityName, string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("MeteoApiKey non trovata nei segreti utente!");
        }

        var url = $"{_apiUrl}/geo/1.0/direct?q={cityName}&limit=1&appid={apiKey}";

        var json = await _httpClient.GetStringAsync(url);
        var apiResponse = JsonSerializer.Deserialize<List<LocationModel>>(json);

        if (apiResponse == null || apiResponse.Count == 0)
            return null;

        return apiResponse[0];
    }


    public async Task<WeatherModel?> GetWeatherAsync(ILocation location, string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("MeteoApiKey non trovata nei segreti utente!");
        }

        var url = $"{_apiUrl}/data/2.5/weather?lat={location.Latitude}&lon={location.Longitude}&appid={apiKey}&units=metric";

        var json = await _httpClient.GetStringAsync(url);
        var apiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(json);

        if (apiResponse == null) 
            return null;

        return new WeatherModel
        {
            Description = apiResponse.Weather.First().Description,
            Temperature = apiResponse.Main.Temperature,
            FeelsLike = apiResponse.Main.FeelsLike
        };
    }

    public async Task<string?> GetNameByPostionAsync(ILocation location, string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("MeteoApiKey non trovata nei segreti utente!");
        }

        var url = $"{_apiUrl}/geo/1.0/reverse?lat={location.Latitude}&lon={location.Longitude}&limit=1&appid={apiKey}";

        var json = await _httpClient.GetStringAsync(url);
        var apiResponse = JsonSerializer.Deserialize<List<GeoResult>>(json);

        if (apiResponse == null) 
            return null;

        return apiResponse[0].Name;
    }
}
