using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using Microsoft.Extensions.Configuration;

namespace MeteoApp.ViewModels
{
    public class MeteoViewModel
    {
        private readonly string _apiKey;
        private readonly IWeatherService _weatherService;

        public MeteoViewModel(IConfiguration configuration, IWeatherService weatherService)
        {
            _apiKey = configuration["MeteoApiKey"];
            _weatherService = weatherService;
        }

        public async Task<WeatherModel> GetWeatherAsync(LocationModel location)
        {
            return await _weatherService.GetWeatherAsync(location, _apiKey);
        }
    }
}
