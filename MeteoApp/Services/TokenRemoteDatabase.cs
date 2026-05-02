using Appwrite;
using Appwrite.Services;
using MeteoApp.Core.Models;
using MeteoApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeteoApp.Services;

public class TokenRemoteDatabase : IDatabase<TokenModel>
{
    private readonly Client _client;
    private readonly Databases _databases;
    private readonly Account _account;

    private readonly string DatabaseId;
    private readonly string CollectionId;

    public TokenRemoteDatabase(IAppConfigProvider configProvider)
    {
        _client = new Client()
            .SetEndpoint(configProvider.GetAppwriteEndpoint())
            .SetProject(configProvider.GetAppwriteProjectId());

        _databases = new Databases(_client);
        _account = new Account(_client);

        DatabaseId = configProvider.GetAppwriteDatabaseId();
        CollectionId = configProvider.GetAppwriteTokenCollectionId();
    }

    public async Task<IEnumerable<TokenModel>> GetDataAsync()
    {
        var tokens = new List<TokenModel>();

        try
        {
            var response = await _databases.ListDocuments(
                databaseId: DatabaseId,
                collectionId: CollectionId
            );

            foreach (var doc in response.Documents)
            {
                tokens.Add(new TokenModel
                {
                    Id = doc.Id,
                    Token = doc.Data.ContainsKey("token") ? doc.Data["token"].ToString() : string.Empty
                });
            }
        }
        catch (AppwriteException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MeteoAppSync] Error GetUpdatedSinceAsync: {ex.Message}");
        }

        return tokens;
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

    public async Task PushDeleteAsync(TokenModel entity)
    {
        await _databases.DeleteDocument(
            databaseId: DatabaseId,
            collectionId: CollectionId,
            documentId: entity.Id
        );
    }

    public async Task PushUpsertAsync(TokenModel entity)
    {
        var documentData = new Dictionary<string, object>
        {
            { "token", entity.Token }
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
            // Token già esiste, nessuna azione richiesta
        }
    }
}
