using Appwrite;
using Appwrite.Services;
using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Services;

public class LocationRemoteDatabase : IRemoteDatabase<LocationModel>
{
    private readonly Client _client;
    private readonly Databases _databases;
    private readonly Account _account;

    private const string DatabaseId = "IL_TUO_DATABASE_ID";
    private const string CollectionId = "IL_TUO_COLLECTION_ID";

    public LocationRemoteDatabase(IAppConfigProvider configProvider)
    {
        _client = new Client()
            .SetEndpoint(configProvider.GetAppwriteEndpoint())
            .SetProject(configProvider.GetAppwriteProjectId());

        _databases = new Databases(_client);
        _account = new Account(_client);
    }

    public Task<IEnumerable<LocationModel>> GetUpdatedSinceAsync(DateTime lastSyncDate)
    {
        throw new NotImplementedException();
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _account.Get();
        }
        catch (AppwriteException ex) when (ex.Code == 401)
        {
            await _account.CreateAnonymousSession();
        }
    }

    public async Task PushDeleteAsync(LocationModel entity)
    {
        await _databases.DeleteDocument(
            databaseId: DatabaseId,
            collectionId: CollectionId,
            documentId: entity.Id
        );
    }

    public async Task PushUpsertAsync(LocationModel entity)
    {
        var documentData = new Dictionary<string, object>
        {
            { "name", entity.Name },
            { "lat", entity.Latitude },
            { "lon", entity.Longitude },
            { "UpdatedAt", entity.UpdatedAt.ToString("o") },
            { "IsDeleted", entity.IsDeleted }
        };

        try
        {
            await _databases.CreateDocument(
                databaseId: DatabaseId,
                collectionId: CollectionId,
                documentId: entity.Id,
                data: documentData
            );
        }
        catch (AppwriteException ex) when (ex.Code == 409) 
        {
            await _databases.UpdateDocument(
                databaseId: DatabaseId,
                collectionId: CollectionId,
                documentId: entity.Id,
                data: documentData
            );
        }
    }
}
