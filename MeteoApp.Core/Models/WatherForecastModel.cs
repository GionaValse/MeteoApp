using System.Text.Json.Serialization;

namespace MeteoApp.Core.Models;

// ── Root ────────────────────────────────────────────────────────────────────

public class WeatherForecastResponse
{
    [JsonPropertyName("cod")]
    public string Cod { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public int Message { get; set; }

    [JsonPropertyName("cnt")]
    public int Count { get; set; }

    [JsonPropertyName("list")]
    public List<ForecastItem> List { get; set; } = [];

    [JsonPropertyName("city")]
    public City City { get; set; } = new();
}

// ── Forecast item (ogni 3 ore) ───────────────────────────────────────────────

public class ForecastItem
{
    /// <summary>Unix timestamp UTC (secondi dal 1970-01-01)</summary>
    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("main")]
    public MainData Main { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<WeatherCondition> Weather { get; set; } = [];

    [JsonPropertyName("clouds")]
    public Clouds Clouds { get; set; } = new();

    [JsonPropertyName("wind")]
    public Wind Wind { get; set; } = new();

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    /// <summary>Probabilità di precipitazioni (0.0 – 1.0)</summary>
    [JsonPropertyName("pop")]
    public double Pop { get; set; }

    [JsonPropertyName("rain")]
    public Rain? Rain { get; set; }

    [JsonPropertyName("sys")]
    public ForecastSys Sys { get; set; } = new();

    /// <summary>Data e ora in formato leggibile (es. "2026-04-17 12:00:00")</summary>
    [JsonPropertyName("dt_txt")]
    public string DtTxt { get; set; } = string.Empty;

    // ── Helper ────────────────────────────────────────────────────────────

    /// <summary>Converte dt in DateTimeOffset locale</summary>
    public DateTimeOffset DateTime =>
        DateTimeOffset.FromUnixTimeSeconds(Dt).ToLocalTime();

    /// <summary>Prima condizione meteo (la più rilevante)</summary>
    public WeatherCondition? PrimaryWeather =>
        Weather.FirstOrDefault();

    /// <summary>Probabilità precipitazioni in percentuale (0–100)</summary>
    public int PrecipitationPercent =>
        (int)(Pop * 100);
}

// ── Main (temperature e pressione) ──────────────────────────────────────────

public class MainData
{
    /// <summary>Temperatura in °C (con units=metric)</summary>
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("temp_min")]
    public double TempMin { get; set; }

    [JsonPropertyName("temp_max")]
    public double TempMax { get; set; }

    /// <summary>Pressione al livello del mare (hPa)</summary>
    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }

    [JsonPropertyName("sea_level")]
    public int SeaLevel { get; set; }

    [JsonPropertyName("grnd_level")]
    public int GroundLevel { get; set; }

    /// <summary>Umidità relativa (%)</summary>
    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("temp_kf")]
    public double TempKf { get; set; }
}

// ── Condizione meteo ─────────────────────────────────────────────────────────

public class WeatherCondition
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>Gruppo (es. "Rain", "Clouds", "Clear")</summary>
    [JsonPropertyName("main")]
    public string Main { get; set; } = string.Empty;

    /// <summary>Descrizione (in italiano con lang=it)</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Codice icona (es. "04d") → usare con https://openweathermap.org/img/wn/{icon}@2x.png</summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    public string IconUrl =>
        $"https://openweathermap.org/img/wn/{Icon}@2x.png";
}

// ── Nuvole ───────────────────────────────────────────────────────────────────

public class Clouds
{
    /// <summary>Copertura nuvolosa (%)</summary>
    [JsonPropertyName("all")]
    public int All { get; set; }
}

// ── Vento ────────────────────────────────────────────────────────────────────

public class Wind
{
    /// <summary>Velocità in m/s (con units=metric)</summary>
    [JsonPropertyName("speed")]
    public double Speed { get; set; }

    /// <summary>Direzione in gradi meteorologici</summary>
    [JsonPropertyName("deg")]
    public int Deg { get; set; }

    [JsonPropertyName("gust")]
    public double Gust { get; set; }
}

// ── Pioggia ──────────────────────────────────────────────────────────────────

public class Rain
{
    /// <summary>Volume pioggia nelle ultime 3 ore (mm)</summary>
    [JsonPropertyName("3h")]
    public double ThreeHours { get; set; }
}

// ── Sys (giorno/notte) ───────────────────────────────────────────────────────

public class ForecastSys
{
    /// <summary>"d" = giorno, "n" = notte</summary>
    [JsonPropertyName("pod")]
    public string Pod { get; set; } = string.Empty;

    public bool IsDay => Pod == "d";
}

// ── Città ─────────────────────────────────────────────────────────────────────

public class City
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("coord")]
    public Coord Coord { get; set; } = new();

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("population")]
    public int Population { get; set; }

    /// <summary>Offset fuso orario in secondi rispetto a UTC</summary>
    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }

    [JsonPropertyName("sunrise")]
    public long Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    public long Sunset { get; set; }

    public DateTimeOffset SunriseTime =>
        DateTimeOffset.FromUnixTimeSeconds(Sunrise).ToLocalTime();

    public DateTimeOffset SunsetTime =>
        DateTimeOffset.FromUnixTimeSeconds(Sunset).ToLocalTime();
}

// ── Coordinate ───────────────────────────────────────────────────────────────

public class Coord
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }
}