using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Models;

public class UserPreferences
{
    public required string Language { get; set; }
    public bool NotificationsEnabled { get; set; }
    public DateTime LastSync { get; set; }
}
