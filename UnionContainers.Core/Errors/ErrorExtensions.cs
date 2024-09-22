namespace UnionContainers.Errors;

public static class ErrorExtensions
{
    public static TError SetMessage<TError>(this TError error, string? message) where TError : struct,IError
    {
        /*error.SetMessage(message);
        return error;*/
        return error with {Message = message};
    }
    
    public static string GetMessage<TError>(this ref TError error) where TError : struct,IError
    {
        return error.GetMessage();
    }
    
}