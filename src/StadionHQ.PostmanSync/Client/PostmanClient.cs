using System.Net;
using System.Text.Json;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;
using Stadion.PostmanSync.Client.Requests;
using Stadion.PostmanSync.Client.Responses;

namespace Stadion.PostmanSync.Client;

public class PostmanClient
{
    private readonly string apiKey;
    private readonly ILogger logger;
    private readonly RestClient client;

    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    
    public PostmanClient(string apiKey, ILogger logger)
    {
        this.apiKey = apiKey;
        this.logger = logger;
        client = new RestClient("https://api.getpostman.com")
        {
            Timeout = -1
        };

        client.UseSystemTextJson(jsonSerializerOptions);
    }

    #region APIs
    public async Task<IEnumerable<Api>> Get()
    {
        var response = await MakeRequestAsync<GetApisResponse>($"apis", Method.GET);
        return response.Apis;
    }

    public async Task<Api> Get(Guid id)
    {
        var response = await MakeRequestAsync<GetApiResponse>($"apis/{id}", Method.GET);
        return response.Api;
    }

    #endregion

    #region Schemas
    
    public async Task<UpdateSchemaResponse> UpdateApiSchema(
        Guid apiId,
        Guid apiVersionId,
        Guid schemaId,
        UpdateApiSchemaRequest request)
    {
        return await MakeRequestAsync<UpdateSchemaResponse>(
            $"apis/{apiId}/versions/{apiVersionId}/schemas/{schemaId}", Method.PUT, request);
    }
    #endregion

    #region Relations

    public async Task<SyncRelationsWithSchemaResponse> SyncRelationsWithSchema(
        Guid apiId,
        Guid apiVersionId,
        string entityType,
        string entityId)
    {
        var url = $"apis/{apiId}/versions/{apiVersionId}/{entityType}/{entityId}/syncWithSchema";
        logger.LogTrace($"Sending to url: {url}");
        return await MakeRequestAsync<SyncRelationsWithSchemaResponse>(url, Method.PUT);
    }
    

    #endregion

   
    private async Task<T> MakeRequestAsync<T>(string resource, Method method, object? jsonBody = null)
    {
        var request = new RestRequest(resource, method);
        request.AddHeader("X-API-Key", apiKey);

        if (jsonBody != null)
        {
            request.AddJsonBody(jsonBody);
        }

        var response = await client.ExecuteAsync(request);
        if (response == null)
        {
            throw new UnexpectedPostmanResponseException("Response was null");
        }

        if (!response.IsSuccessful)
        {
            if(response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
            {
                // var errorResult = JsonSerializer.Deserialize<PostmanErrorResponse>(response.Content, jsonSerializerOptions);
                throw new UnexpectedPostmanResponseException("Non OK status code", response.Content, response.StatusCode);
                // throw new PostmanClientException($"Postman response is {response.StatusCode}: {errorResult?.Error.Name} - {errorResult?.Error.Message}", errorResult?.Error);
            }

            throw new UnexpectedPostmanResponseException("Unexpected response from Postman server", response.Content, response.StatusCode);
        }

        var result = JsonSerializer.Deserialize<T>(response.Content, jsonSerializerOptions);

        if (result == null)
        {
            throw new UnexpectedPostmanResponseException("Deserialized response was null", response.Content, response.StatusCode);
        }

        return result;
    }
}