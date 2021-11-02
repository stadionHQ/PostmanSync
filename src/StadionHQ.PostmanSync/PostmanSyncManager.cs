using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Stadion.PostmanSync.Client;
using Stadion.PostmanSync.Client.Requests;

namespace Stadion.PostmanSync;

public interface IPostmanSyncManager
{
    Task RunAsync();
}

public class PostmanSyncManager: IPostmanSyncManager
{
    private readonly PostmanClient client;
    private readonly PostmanSyncOptions options;
    private readonly ILogger<PostmanSyncManager> logger;
    public const string OpenApi3 = "openapi3";

    public PostmanSyncManager(
        IOptions<PostmanSyncOptions> options, 
        ILogger<PostmanSyncManager> logger)
    {
        if (options.Value == default || options.Value.PostmanApiKey == default || options.Value.Profiles == default)
        {
            throw new PostmanSyncException("PostmanSync configuration is null. Check your configuration setup");
        }
        
        
        this.options = options.Value;
        AssertUniqueKeys();

        client = new PostmanClient(options.Value.PostmanApiKey);
        this.logger = logger;
    }


    /// <summary>
    /// Start processing the profile defined in the configuration
    /// </summary>
    public async Task RunAsync()
    {
        Console.WriteLine("   ___          _                           __                  ");
        Console.WriteLine(@"  / _ \___  ___| |_ _ __ ___   __ _ _ __   / _\_   _ _ __   ___ ");
        Console.WriteLine(@" / /_)/ _ \/ __| __| '_ ` _ \ / _` | '_ \  \ \| | | | '_ \ / __|");
        Console.WriteLine(@"/ ___/ (_) \__ \ |_| | | | | | (_| | | | | _\ \ |_| | | | | (__ ");
        Console.WriteLine(@"\/    \___/|___/\__|_| |_| |_|\__,_|_| |_| \__/\__, |_| |_|\___|");
        Console.WriteLine(@"                                               |___/");
        logger.LogInformation("Postman Sync has started");
        foreach (var profile in options.Profiles)
        {
            await ProcessProfileAsync(profile);
        }
        logger.LogInformation("✅ Postman Sync has finished");
        logger.LogInformation("");
    }

    public async Task ProcessProfileAsync(PostmanSyncProfile profile)
    {
        logger.LogInformation($"Processing profile '{profile.Key}'");

        var sourceSchema = profile.SourceSchema;
            
        if (sourceSchema == null)
        {
            throw new PostmanSyncException($"Source Schema is missing from profile: {JsonSerializer.Serialize(profile)}");
        }
        
        var schemaContent = FetchContentString(sourceSchema.Url);

        if (string.IsNullOrEmpty(schemaContent))
        {
            throw new PostmanSyncException(
                $"Attempted to fetch source schema content from '{sourceSchema.Url}', but no content was returned");
        }

        await UpdateApiSchemaAsync(profile, schemaContent);

        logger.LogInformation($"Updated the api schema for profile '{profile.Key}'");
        
        foreach (var relation in profile.Relations)
        {
            // These could be run in parallel
            await SyncApiSchemaToCollection(profile.ApiId, profile.VersionId, relation);
        }
    }

    /// <summary>
    /// Updates a Postman API definition with the passed sync profile. The exact api, version and schema
    /// updated is defined by the profile.
    /// </summary>
    /// <param name="profile">A profile representing a Postman api to update</param>
    /// <param name="sourceSchema">The string content of the source schema to update the api with.</param>
    public async Task UpdateApiSchemaAsync(PostmanSyncProfile profile, string sourceSchema)
    {
        var updateRequest = new UpdateApiSchemaRequest(new UpdateSchema()
        {
            Language = "json",
            Type = OpenApi3,
            Schema = sourceSchema
        });
        
        await client.UpdateApiSchema(profile.ApiId, profile.VersionId, profile.SchemaId, updateRequest);
    }

    public async Task SyncApiSchemaToCollection(Guid apiId, Guid versionId, PostmanSyncRelation relation)
    {
        logger.LogInformation($"Syncing Postman api schema updates to relation {relation.EntityType}/{relation.EntityId}");
        await client.SyncRelationsWithSchema(apiId, versionId, relation.EntityType, relation.EntityId);
        logger.LogInformation("Syncing the relation has finished.");
    }   

    /// <summary>
    /// Returns string content from the passed URL
    /// </summary>
    private static string FetchContentString(string url)
    {
        using var webClient = new WebClient();
        var s = webClient.DownloadString(url);
        return s;
    }
    
    /// <summary>
    /// Checks that all profiles defined in the configuration have a unique key.
    /// </summary>
    /// <exception cref="PostmanSyncException">Detected profiles with duplicate keys</exception>
    private void AssertUniqueKeys()
    {
        var allProfileKeys = options.Profiles.Select(u => u.Key);
        var profileKeys = allProfileKeys as string[] ?? allProfileKeys.ToArray();
        var distinctKeys = profileKeys.Distinct();
        if (profileKeys.Count() != distinctKeys.Count())
        {
            throw new PostmanSyncException("PostmanSync configuration contains profiles with duplicate keys");
        }
    }
}