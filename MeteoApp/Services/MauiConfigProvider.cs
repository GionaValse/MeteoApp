using MeteoApp.Core.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Services;

public class MauiConfigProvider : IAppConfigProvider
{
    private readonly IConfiguration _configuration;

    public MauiConfigProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetAppwriteApiKey()
    {
        return _configuration["AppwriteApiKey"];
    }

    public string GetAppwriteEndpoint()
    {
        return _configuration["AppwriteEndpoint"];
    }

    public string GetAppwriteProjectId()
    {
        return _configuration["AppwriteProjectId"];
    }

    public string GetWeatherApiKey()
    {
        return _configuration["MeteoApiKey"];
    }

    public string GetAppwriteDatabaseId()
    {
        return _configuration["AppwriteDatabaseId"];
    }

    public string GetAppwriteLocationCollectionId()
    {
        return _configuration["AppwriteLocationCollectionId"];
    }

    public string GetAppwriteTokenCollectionId()
    {
        return _configuration["AppwriteTokenCollectionId"];
    }
}
