using System.Net;

namespace Stadion.PostmanSync.Client;

public class PostmanError
{
    public string Name { get; set; }
    public string Message { get; set; }
}

public class PostmanErrorResponse
{
    public PostmanError Error { get; set; }
}

public class UnexpectedPostmanResponseException: Exception
{
    public UnexpectedPostmanResponseException(string? message) : base(message)
    {
    }
    
    public UnexpectedPostmanResponseException(string? message, string responseContent, HttpStatusCode statusCode) : base(message)
    {
        ResponseContent = responseContent;
        ResponseStatusCode = statusCode;
    }
    // public PostmanClientException(string? message, PostmanError error) : base(message)
    // {
    //     Error = error;
    // }

    // public PostmanError? Error { get; }
    public string ResponseContent { get; }
    public HttpStatusCode ResponseStatusCode { get; }
}