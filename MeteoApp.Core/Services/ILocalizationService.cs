using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Core.Services;

public interface ILocalizationService
{
    void SetLanguage(string language);
    string GetLanguageCode(string language);
}
