using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MeteoApp.Core.Models;

public class TokenModel : IDatabaseEntity
{
    [PrimaryKey]
    [JsonPropertyName("$id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("token")]
    public string Token { get; set; }
}
