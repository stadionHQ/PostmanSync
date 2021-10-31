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
public class PostmanSyncOptions
{
    public const string PostmanSync = "PostmanSync";
    public string PostmanApiKey { get; set; }
    public IEnumerable<PostmanSyncProfile> Profiles { get; set; }

}