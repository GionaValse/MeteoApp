using MeteoApp.Core.Models;
using MeteoApp.Core.Services;

namespace MeteoApp.ViewModels
{
    public class MeteoViewModel
    {
        private readonly IWeatherService _weatherService;

        public MeteoViewModel(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public async Task<WeatherModel> GetWeatherAsync(LocationModel location)
        {
            return await _weatherService.GetWeatherAsync(location);
        }
    }
}
