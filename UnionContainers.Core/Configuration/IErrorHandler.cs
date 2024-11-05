using HelpfulTypesAndExtensions;

namespace UnionContainers;

public interface IErrorHandler
{
    void HandleError(IError error);
}

public interface IErrorHandler<TError> : IErrorHandler
where TError : struct, IError
{
    void HandleError(TError error);
}

public class ErrorHandler<TError> : IErrorHandler<TError> where TError : struct, IError
{
    private readonly Action<TError>? _errorHandler;
    
    public ErrorHandler(Action<TError> errorHandler)
    {
        _errorHandler = errorHandler;
    }
    
    public virtual void HandleError(IError error)
    {
        if (error is TError errorAsTError)
        {
            _errorHandler?.Invoke(errorAsTError);
        }
    }

    public virtual void HandleError(TError error)
    {
        _errorHandler?.Invoke(error);
    }
}