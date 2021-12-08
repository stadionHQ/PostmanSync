namespace Stadion.PostmanSync;

public class PostmanSyncException: Exception
{
    public PostmanSyncException(string? message) : base(message)
    {
    }
}

public class PostmanApiCallException : Exception
{
    
}