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

        // Already filer for 24 hours, but can be extended to 5 days
        // this return range of 3 hours
        public async Task<List<ForecastItem>> GetForecastAsync(LocationModel location)
        {
            var ris = await _weatherService.GetForecastDataAsync(location);
            List<ForecastItem> filteredList = new List<ForecastItem>();
            ris.List.ForEach(item =>
            {
                DateTime dt = DateTime.Parse(item.DtTxt);
                if (dt > DateTime.Now && dt < DateTime.Now.AddHours(24))
                    filteredList.Add(item);
            });
            filteredList.ForEach(i => i.Main.Temp = (double)(i.Main.Temp - 273.15)); // Convert from Kelvin to Celsius
            return filteredList;
        }
    }
}
