using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public interface INotificationProvider
{
    Task<string> RequestTokenAsync();
}
