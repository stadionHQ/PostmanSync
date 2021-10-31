namespace Stadion.PostmanSync.Client;

public class Relation
{
    public Guid Id { get; set; }
    public string Model { get; set; }
    public string Type { get; set; }
    public string ModelId { get; set; }
}

public class ApiSchema
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Language { get; set; }
    public string Schema { get; set; }
    public Guid ApiVersion { get; set; }
}

public class ApiVersion
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<ApiSchema> Schemas { get; set; }
    public IEnumerable<Relation> Relations { get; set; }
}

public class Api
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }
    public IEnumerable<ApiVersion> Versions { get; set; }
}

public class GetApiResponse
{
    public Api Api { get; set; }
}

public class GetApisResponse
{
    public IEnumerable<Api> Apis { get; set; }
}