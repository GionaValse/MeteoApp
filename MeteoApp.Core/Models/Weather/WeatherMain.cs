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
}
