using HelpfulTypesAndExtensions;

namespace UnionContainers;

public class UnionContainerOptions
{
    public UnionContainerErrorService ErrorService { get; } = new();
    internal static UnionContainerErrorService InternalErrorService { get; set; } = new();

    public UnionContainerOptions(Action<UnionContainerErrorService>? configureErrorService = null)
    {
        ErrorService = new UnionContainerErrorService();
        configureErrorService?.Invoke(ErrorService);
        InternalErrorService = ErrorService;
    }
    
    public void AddDefaultErrorConverters() => ErrorService.AddDefaultErrorConverters();

    public bool TryRegisterErrorHandler<TError>(IErrorHandler errorHandler) where TError : struct, IError => ErrorService.TryRegisterErrorHandler<TError>(errorHandler);
}

public record UCErrorConvertorDescription(Type ExceptionType, Type ErrorType, IErrorConverter ErrorConverter);

public class UnionContainerErrorService 
{
    /// <summary>
    /// Key: the name of the Exception to convert FROM <br/>
    /// Value: the function that converts the Exception to an IError
    /// </summary>
    internal static Dictionary<string, UCErrorConvertorDescription> ErrorConverters { get; } = new();
    
    internal static Dictionary<string,IErrorHandler> ErrorHandlers { get; } = new();
    
    
    public void RegisterErrorConverter<TException, TError>(IErrorConverter<TException, TError> errorConverter) where TException : Exception where TError : struct, IError
    {
        if(ErrorConverters.ContainsKey(nameof(TException)))
        {
            throw new InvalidOperationException($"An error converter for {typeof(TException).Name} has already been registered.");
        }
        var errorConvertorDescription = new UCErrorConvertorDescription(typeof(TException), typeof(TError), errorConverter);
        ErrorConverters.Add(nameof(TException),errorConvertorDescription);
    }
    
    public void RegisterErrorConverter(UCErrorConvertorDescription errorConvertorDescription)
    {
        if(ErrorConverters.ContainsKey(errorConvertorDescription.ExceptionType.Name))
        {
            throw new InvalidOperationException($"An error converter for {errorConvertorDescription.ExceptionType.Name} has already been registered.");
        }
        ErrorConverters.Add(errorConvertorDescription.ExceptionType.Name,errorConvertorDescription);
    }
    
    public bool TryRegisterErrorConverter<TException, TError>(IErrorConverter<TException, TError> errorConverter) where TException : Exception where TError : struct, IError
    {
        try
        {
            RegisterErrorConverter(errorConverter);
            return true;
        }
        catch (InvalidOperationException e)
        {
            return false;
        }
    }
    
    public bool TryRegisterErrorConverter(UCErrorConvertorDescription errorConvertorDescription)
    {
        try
        {
            RegisterErrorConverter(errorConvertorDescription);
            return true;
        }
        catch (InvalidOperationException e)
        {
            return false;
        }
    }
    
    public UnionContainer<IErrorConverter<TException,TError>> TryGetErrorConverter<TException, TError>() where TException : Exception where TError : struct, IError
    {
        try
        {
            if(ErrorConverters.TryGetValue(nameof(TException), out var errorConvertorDescription))
            {
                var matchingErrorConvertor = (IErrorConverter<TException, TError>)errorConvertorDescription.ErrorConverter;
                return new UnionContainer<IErrorConverter<TException, TError>>(matchingErrorConvertor);
            }
            return new UnionContainer<IErrorConverter<TException, TError>>();
        }
        catch (Exception e)
        {
            return e;
        }
    }
    
    internal bool TryConvertException(Exception exception, out IError error)
    {
        if(ErrorConverters.TryGetValue(exception.GetType().Name, out var errorConvertorDescription))
        {
            error = errorConvertorDescription.ErrorConverter.Convert(exception);
            return true;
        }
        error = new CustomErrors.ExceptionWrapperError(exception);
        return false;
    }
    
    /// <summary>
    /// Adds the default error converters to the service
    /// </summary>
    public void AddDefaultErrorConverters()
    {
        List<IErrorConverter> defaultErrorConverters =
        [
            new ErrorConverter<NullReferenceException, ClientErrors.ValidationFailureError>
            (
                 exception => new ClientErrors.ValidationFailureError()
                 {
                     Message = $"The {exception.TargetSite} method returned a null reference.",
                     Source = exception.Source + exception.TargetSite
                 }
            ),
            new ErrorConverter<ArgumentNullException, ClientErrors.ValidationFailureError>
            (
                exception => new ClientErrors.ValidationFailureError()
                {
                    Message = $"The {exception.ParamName} parameter in method {exception.TargetSite} is null.",
                    Source = exception.Source + exception.TargetSite
                }
            ),
            new ErrorConverter<UriFormatException,ServerErrors.PreconditionFailedError>
            (
                exception => new ServerErrors.PreconditionFailedError()
                {
                    Message = $"The URI was incorrectly formatted the following reason was provided: {exception.Message}",
                    Source = exception.Source + exception.TargetSite,
                    PriorityLevel = ErrorSeverity.High
                }
            ),
            new ErrorConverter<HttpRequestException,NetworkErrors.NetworkingError>
            (
                exception => new NetworkErrors.NetworkingError()
                {
                    Message = $"An error occurred while sending the request.\n\t Reason: {exception.Message}",
                    Source = exception.Source + exception.TargetSite,
                    PriorityLevel = ErrorSeverity.High
                }
            ),
            new ErrorConverter<InvalidOperationException,ClientErrors.InvalidOperationError>
            (
                exception => new ClientErrors.InvalidOperationError()
                {
                    Message = $"An invalid operation occurred. The following reason was provided: {exception.Message}",
                    Source = exception.Source + exception.TargetSite,
                    PriorityLevel = ErrorSeverity.High
                }
            )
        ];
        defaultErrorConverters.ForEachIfNotNull(errorConverter => TryRegisterErrorConverter(errorConverter.ToUCErrorConvertorDescription()));
    }
    
    public void RegisterErrorHandler<TError>(IErrorHandler errorHandler) where TError : struct, IError
    {
        if(ErrorHandlers.TryAdd(typeof(TError).Name, errorHandler) is false)
        {
            throw new InvalidOperationException($"An error handler for {typeof(TError).Name} has already been registered.");
        }
        Console.WriteLine($"Error handler for {typeof(TError).Name} registered successfully.");
    }
    
    public bool TryRegisterErrorHandler<TError>(IErrorHandler errorHandler) where TError : struct, IError
    {
        try
        {
            RegisterErrorHandler<TError>(errorHandler);
            return true;
        }
        catch (InvalidOperationException e)
        {
            return false;
        }
    }
    
    public UnionContainer<IErrorHandler> TryGetErrorHandler<TError>() where TError : struct, IError
    {
        try
        {
            if(ErrorHandlers.TryGetValue(nameof(TError), out var errorHandler))
            {
                return new UnionContainer<IErrorHandler>(errorHandler);
            }
            return new UnionContainer<IErrorHandler>();
        }
        catch (Exception e)
        {
            return e;
        }
    }
    
    internal bool TryHandleError(IError error)
    {
        DebugHelp.DebugWriteLine($"Handling error of type {error.GetType().Name}");
        foreach (var errorHandlerpair in ErrorHandlers)
        {
            DebugHelp.DebugWriteLine($"Error handler for {errorHandlerpair.Key} found.");
        }
        if(ErrorHandlers.TryGetValue(error.GetType().Name, out var errorHandler))
        {
            errorHandler.HandleError(error);
            DebugHelp.DebugWriteLine($"Error of type {error.GetType().Name} handled successfully.");
            return true;
        }
        DebugHelp.DebugWriteLine($"Error of type {error.GetType().Name} could not be handled.");
        return false;
    }
}