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

public class PostmanException: Exception
{
    public PostmanException(string? message) : base(message)
    {
    }
    
    public PostmanException(string? message, string responseContent) : base(message)
    {
        ResponseContent = responseContent;
    }
    public PostmanException(string? message, PostmanError error) : base(message)
    {
        Error = error;
    }

    public PostmanError? Error { get; }
    public string ResponseContent { get; }
}