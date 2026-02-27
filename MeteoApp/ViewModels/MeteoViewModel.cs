using Microsoft.Extensions.Configuration;

namespace MeteoApp.ViewModels
{
    public class MeteoViewModel
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public MeteoViewModel(IConfiguration configuration, HttpClient httpClient)
        {
            _apiKey = "";

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("MeteoApiKey non trovata nei segreti utente!");
            }

            _httpClient = httpClient;
        }

        public async Task<string> GetWeatherAsync(string city)
        {
            try
            {
                var url = $"https://api.openweathermap.org/data/2.5/weather?lat=44.34&lon=10.99&appid={_apiKey}";

                var response = await _httpClient.GetStringAsync(url);

                Console.WriteLine($"Meteo response: {response}");

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante la chiamata API: {ex.Message}");
                return null;
            }
        }
    }
}
