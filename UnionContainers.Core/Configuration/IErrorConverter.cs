using HelpfulTypesAndExtensions;

namespace UnionContainers;

public interface IErrorConverter
{
    IError Convert(Exception exception);
}

public interface IErrorConverter<in TException, out TError> : IErrorConverter 
where TException : Exception where TError : struct, IError
{
    TError Convert(TException exception);
}

public abstract class ErrorConverterBase<TException, TError> : IErrorConverter<TException, TError> where TException : Exception where TError : struct, IError
{
    public abstract TError Convert(TException exception);

    /// <inheritdoc />
    public IError Convert(Exception exception) => Convert((TException)exception);
}

public class ErrorConverter<TException, TError> : ErrorConverterBase<TException, TError> where TException : Exception where TError : struct, IError
{
    private readonly Func<TException, TError> _convertFunc;

    public ErrorConverter(Func<TException, TError> convertFunc)
    {
        _convertFunc = convertFunc;
    }

    public override TError Convert(TException exception)
    {
        return _convertFunc(exception);
    }
}


public static class ErrorConverterExtensions
{
    public static UCErrorConvertorDescription ToUCErrorConvertorDescription<TException, TError>(this IErrorConverter<TException, TError> errorConverter) where TException : Exception where TError : struct, IError
    {
        return new UCErrorConvertorDescription(typeof(TException), typeof(TError), errorConverter);
    }
    
    public static UCErrorConvertorDescription ToUCErrorConvertorDescription(this IErrorConverter errorConverter)
    {
        return new UCErrorConvertorDescription(errorConverter.GetType().GetGenericArguments()[0], errorConverter.GetType().GetGenericArguments()[1], errorConverter);
    }
}