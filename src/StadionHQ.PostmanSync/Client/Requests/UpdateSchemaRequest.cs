namespace Stadion.PostmanSync.Client.Requests;

public class UpdateSchema
{
    public string Type { get; set; }
    public string Language { get; set; }
    public string Schema { get; set; }
}

public class UpdateApiSchemaRequest
{
    public UpdateApiSchemaRequest(UpdateSchema schema)
    {
        Schema = schema;
    }

    public UpdateSchema Schema { get; }
}