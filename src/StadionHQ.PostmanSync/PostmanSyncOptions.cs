namespace Stadion.PostmanSync;
public class PostmanSyncRelation
{
    public string EntityType { get; set; }
    public string EntityId { get; set; }
}

public class PostmanSyncSourceSchema
{
    public string Type { get; set; }
    public string Url { get; set; }
}


/// <summary>
/// Models an Postman API that will be synced automatically.
/// </summary>
public class PostmanSyncProfile
{
    public PostmanSyncSourceSchema SourceSchema { get; set; }
    
    /// <summary>
    /// A unique identifier for this profile.
    /// Must be in the format
    /// "{apiName}_{versionName}_{schemaType}"
    /// </summary>
    public string Key { get; set; }
    public Guid ApiId { get; set; }
    public Guid VersionId { get; set; }
    public Guid SchemaId { get; set; }
    
    /// <summary>
    /// All the entities related to the API that will be synced as part of the schema update.
    /// If you leave this empty, the API schema will be updated but no sync between Documentation, Tests,
    /// Monitors, etc will take place, so they will stay out of date.
    /// </summary>
    public IEnumerable<PostmanSyncRelation> Relations { get; set; }
}

public class PostmanSyncHostedServiceOptions
{
    /// <summary>
    /// Enable or disable the hosted service from running
    /// </summary>
    public bool Enabled { get; set; }
    
    /// <summary>
    /// If true, will allow the hosted service to raise exceptions that occur
    /// during the sync on startup. This will cause the .NET core host to crash
    /// when postman syncing fails. To prevent this, set this to false.
    /// </summary>
    public bool ThrowExceptions { get; set; }
}


public class PostmanSyncOptions
{
    public const string PostmanSync = "PostmanSync";
    public string PostmanApiKey { get; set; }
    public IEnumerable<PostmanSyncProfile> Profiles { get; set; }

    /// <summary>
    /// Configuration to enable a hosted service to be installed which will automatically run postman sync on startup
    /// </summary>
    public PostmanSyncHostedServiceOptions? HostedService { get; set; }
}