using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Models;

public interface IDatabaseEntity
{
    string Id { get; set; }
}
