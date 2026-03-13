using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MeteoApp.Core.Models.Weather;

public class WeatherDescription
{
    [JsonPropertyName("description")]
    public string Description { get; set; }
}
