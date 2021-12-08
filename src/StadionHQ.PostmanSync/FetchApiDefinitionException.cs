namespace Stadion.PostmanSync;

public class FetchApiDefinitionException: Exception
{
    public FetchApiDefinitionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}