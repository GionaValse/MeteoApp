using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Models;

public class WeatherModel
{
    public string Description { get; set; }
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
}
