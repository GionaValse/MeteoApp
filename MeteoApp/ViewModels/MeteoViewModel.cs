using MeteoApp.Core.Models;
using MeteoApp.Core.Services;

namespace MeteoApp.ViewModels
{
    public class MeteoViewModel
    {
        private readonly IAppConfigProvider _config;
        private readonly IWeatherService _weatherService;

        public MeteoViewModel(IAppConfigProvider configuration, IWeatherService weatherService)
        {
            _config = configuration;
            _weatherService = weatherService;
        }

        public async Task<WeatherModel> GetWeatherAsync(LocationModel location)
        {
            var apiKey = _config.GetWeatherApiKey();
            return await _weatherService.GetWeatherAsync(location, apiKey);
        }
    }
}
