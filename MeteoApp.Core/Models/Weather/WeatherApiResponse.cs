using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Models.Weather;

public class WeatherApiResponse
{
    public WeatherMain main { get; set; }
    public List<WeatherDescription> weather { get; set; }
}
