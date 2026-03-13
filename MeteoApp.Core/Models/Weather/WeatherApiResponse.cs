using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MeteoApp.Core.Models.Weather;

public class WeatherApiResponse
{
    [JsonPropertyName("weather")]
    public List<WeatherDescription> Weather { get; set; }

    [JsonPropertyName("main")]
    public WeatherMain Main { get; set; }
}
