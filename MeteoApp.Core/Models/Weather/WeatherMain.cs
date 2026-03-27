using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MeteoApp.Core.Models.Weather;

public class WeatherMain
{
    [JsonPropertyName("temp")]
    public double Temperature { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("temp_min")]
    public double TemperatureMin { get; set; }

    [JsonPropertyName("temp_max")]
    public double TemperatureMax { get; set; }
}
