using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Models;

public interface ILocation
{
    public double Latitude { get; }
    public double Longitude { get; }
}
