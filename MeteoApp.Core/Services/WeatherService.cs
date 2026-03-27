using MeteoApp.Core.Models;
using MeteoApp.Core.Models.Weather;
using System.Globalization;
using System.Text.Json;

namespace MeteoApp.Core.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IAppConfigProvider _appConfigProvider;
    private readonly string _apiUrl;

    public WeatherService(HttpClient cllient, IAppConfigProvider appConfigProvider)
    {
        _httpClient = cllient;
        _apiUrl = "https://api.openweathermap.org";
        _appConfigProvider = appConfigProvider;
    }

    public async Task<LocationModel?> GetLocationByNameAsync(string cityName)
    {
        var url = $"{_apiUrl}/geo/1.0/direct?q={cityName}&limit=1";
        var apiResponse = await SendRequestAsync<List<LocationModel>>(url);

        if (apiResponse == null || apiResponse.Count == 0)
            return null;

        return apiResponse[0];
    }

    public async Task<LocationModel?> GetLocationByLatLonAsync(float lat, float lon)
    {
        var url = $"{_apiUrl}/geo/1.0/reverse?lat={lat}&lon={lon}&limit=1";
        var apiResponse = await SendRequestAsync<List<LocationModel>>(url);

        if (apiResponse == null || apiResponse.Count == 0)
            return null;

        return apiResponse[0];
    }

    public async Task<WeatherModel?> GetWeatherAsync(ILocation location)
    {
        var url = $"{_apiUrl}/data/2.5/weather?lat={location.Latitude}&lon={location.Longitude}&units=metric";
        var apiResponse = await SendRequestAsync<WeatherApiResponse>(url);

        if (apiResponse == null) 
            return null;

        return new WeatherModel
        {
            Description = apiResponse.Weather.First().Description,
            Temperature = apiResponse.Main.Temperature,
            FeelsLike = apiResponse.Main.FeelsLike,
            TemperatureMin = apiResponse.Main.TemperatureMin,
            TemperatureMax = apiResponse.Main.TemperatureMax
        };
    }

    public async Task<string?> GetNameByPostionAsync(ILocation location)
    {
        var url = $"{_apiUrl}/geo/1.0/reverse?lat={location.Latitude}&lon={location.Longitude}&limit=1";
        var apiResponse = await SendRequestAsync<List<GeoResult>>(url);

        if (apiResponse == null) 
            return null;

        return apiResponse[0].Name;
    }

    private async Task<T?> SendRequestAsync<T>(string requestUrl)
    {
        string apiKey = _appConfigProvider.GetWeatherApiKey();
        string lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("MeteoApiKey not found in user's secrets!");
        }

        var url = $"{requestUrl}&lang={lang}&appid={apiKey}";

        var json = await _httpClient.GetStringAsync(url);
        var apiResponse = JsonSerializer.Deserialize<T>(json);

        return apiResponse;
    }
}
