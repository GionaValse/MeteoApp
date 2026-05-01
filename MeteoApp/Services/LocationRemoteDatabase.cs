using Appwrite;
using Appwrite.Services;
using MeteoApp.Core.Models;
using MeteoApp.Core.Services;

namespace MeteoApp.Services;

public class LocationRemoteDatabase : ISyncableRemoteDatabase<LocationModel>
{
    private readonly Client _client;
    private readonly Databases _databases;
    private readonly Account _account;

    private readonly string DatabaseId;
    private readonly string CollectionId;

    public LocationRemoteDatabase(IAppConfigProvider configProvider)
    {
        _client = new Client()
            .SetEndpoint(configProvider.GetAppwriteEndpoint())
            .SetProject(configProvider.GetAppwriteProjectId());

        _databases = new Databases(_client);
        _account = new Account(_client);

        DatabaseId = configProvider.GetAppwriteDatabaseId();
        CollectionId = configProvider.GetAppwriteLocationCollectionId();
    }

    public async Task<IEnumerable<LocationModel>> GetDataAsync()
    {
        return await FetchDataAsync(new List<string>());
    }

    public async Task<IEnumerable<LocationModel>> GetUpdatedSinceAsync(DateTime lastSyncDate)
    {
        List<string> queries = new List<string> 
        { 
            Query.GreaterThan("updateTime", lastSyncDate.ToString("o")),
            Query.Limit(100)
        };
        return await FetchDataAsync(queries);
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
            { "updateTime", entity.UpdatedAt.ToString("o") },
            { "isDeleted", entity.IsDeleted }
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

    private async Task<IEnumerable<LocationModel>> FetchDataAsync(List<string> queries)
    {
        var locations = new List<LocationModel>();

        try
        {
            var response = await _databases.ListDocuments(
                databaseId: DatabaseId,
                collectionId: CollectionId,
                queries: queries
            );

            foreach (var doc in response.Documents)
            {
                locations.Add(new LocationModel
                {
                    Id = doc.Id,
                    Name = doc.Data.ContainsKey("name") ? doc.Data["name"].ToString() : string.Empty,
                    Latitude = doc.Data.ContainsKey("lat") ? Convert.ToDouble(doc.Data["lat"]) : 0,
                    Longitude = doc.Data.ContainsKey("lon") ? Convert.ToDouble(doc.Data["lon"]) : 0,
                    UpdatedAt = doc.Data.ContainsKey("updateTime") ? DateTime.Parse(doc.Data["updateTime"].ToString()) : DateTime.MinValue,
                    IsDeleted = doc.Data.ContainsKey("isDeleted") && Convert.ToBoolean(doc.Data["isDeleted"]),

                    NeedsSync = false
                });
            }
        }
        catch (AppwriteException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MeteoAppSync] Error GetUpdatedSinceAsync: {ex.Message}");
        }

        return locations;
    }
}
