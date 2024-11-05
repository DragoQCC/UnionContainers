using HelpfulTypesAndExtensions;

namespace UnionContainers;

public record struct UnionContainer<T1> : IUnionContainerResult<ValueTuple<T1>>
{
    internal UnionContainerState State { get; set; }
    
    internal ValueTuple<T1> ResultValue { get; init; }
    
    internal List<IError>? Errors { get; set; }

    /// <inheritdoc/>
    UnionContainerState IUnionContainer.State
    {
        get => State;
        set => State = value;
    }

    /// <inheritdoc/>
    ValueTuple<T1> IUnionContainerResult<ValueTuple<T1>>.ResultValue
    {
        get => ResultValue;
        init => ResultValue = value;
    }

    /// <inheritdoc/>
    List<IError>? IUnionContainer.Errors
    {
        get => Errors;
        set => Errors = value;
    }

    /// <summary>
    /// Creates an empty <see cref="UnionContainer{T1}"/>.
    /// </summary>
    /// <returns>An empty <see cref="UnionContainer{T1}"/>.</returns>
    public static UnionContainer<T1> EmptyContainer() => new();

    /// <summary>
    /// Creates a <see cref="UnionContainer{T1}"/> with a single error.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error to add to the container.</param>
    /// <returns>A <see cref="UnionContainer{T1}"/> containing the error.</returns>
    public static UnionContainer<T1> ErrorContainer<TError>(TError error)
    where TError : struct, IError
    {
        UnionContainer<T1> container = new();
        container.AddError(error);
        return container;
    }

    /// <summary>
    /// Creates a <see cref="UnionContainer{T1}"/> with a result value.
    /// </summary>
    /// <param name="value">The result value.</param>
    /// <returns>A <see cref="UnionContainer{T1}"/> containing the result value.</returns>
    public static UnionContainer<T1> ResultContainer(T1 value) => new(value);
    
    /// <summary>
    /// Creates a <see cref="UnionContainer{T1}"/> with a single error that wraps the exception.
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static UnionContainer<T1> ExceptionContainer(Exception ex) => new(ex);

    /// <summary>
    /// Converts a method to a <see cref="UnionContainer{T1}"/>.
    /// </summary>
    /// <param name="method">The method to convert.</param>
    /// <param name="parameters">The parameters to pass to the method.</param>
    /// <returns>A <see cref="UnionContainer{T1}"/> containing the result of the method or an error.</returns>
    public static UnionContainer<T1> MethodToContainer(Delegate method, params object[] parameters)
    {
        try
        {
            object? methodResult = method.DynamicInvoke(parameters);
            if (methodResult is T1 newResult)
            {
                return newResult;
            }
        }
        catch (Exception e)
        {
            return e;
        }
        return new UnionContainer<T1>();
    }

    /// <summary>
    /// Converts a function to a <see cref="UnionContainer{T1}"/>.
    /// </summary>
    /// <param name="method">The function to convert.</param>
    /// <returns>A <see cref="UnionContainer{T1}"/> containing the result of the function or an error.</returns>
    public static UnionContainer<T1> MethodToContainer(Func<T1?> method)
    {
        try
        {
            T1? result = method();
            if (result is not null)
            {
                return result;
            }
        }
        catch (Exception e)
        {
            return e;
        }
        return new UnionContainer<T1>();
    }
    

    /// <summary>
    /// Converts an asynchronous function to a <see cref="UnionContainer{T1}"/>.
    /// </summary>
    /// <param name="method">The asynchronous function to convert.</param>
    /// <returns>A <see cref="UnionContainer{T1}"/> containing the result of the function or an error.</returns>
    public static async Task<UnionContainer<T1>> MethodToContainer(Func<Task<T1?>> method)
    {
        try
        {
            T1? result = await method();
            if (result is not null)
            {
                return result;
            }
        }
        catch (Exception e)
        {
            return e;
        }
        return new UnionContainer<T1>();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UnionContainer{T1}"/> struct with a result value.
    /// </summary>
    /// <param name="value">The result value.</param>
    public UnionContainer(T1? value)
    {
        if (value is not null)
        {
            ResultValue = new ValueTuple<T1>(value);
            State = UnionContainerState.Result;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnionContainer{T1}"/> struct with a list of errors.
    /// </summary>
    /// <param name="error">The errors to add to the container.</param>
    public UnionContainer(params IError[] error)
    {
        foreach (IError e in error)
        {
            Errors ??= new List<IError>();
            Errors.Add(e);
            State = UnionContainerState.Error;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnionContainer{T1}"/> struct with an exception.
    /// </summary>
    /// <param name="ex">The exception to wrap in an error.</param>
    public UnionContainer(Exception ex)
    {
        if (UnionContainerOptions.InternalErrorService?.TryConvertException(ex, out IError error) is true)
        {
            Console.WriteLine($"Error converted from previous exception {ex.GetType().Name} to {error.GetType().Name}");
            Errors ??= new List<IError>();
            Errors.Add(error);
            State = UnionContainerState.Error;
        }
        else
        {
            Errors ??= new List<IError>();
            Errors.Add(new CustomErrors.ExceptionWrapperError(ex));
            State = UnionContainerState.Exception;
        }
    }


    /// <summary>
    /// Executes an action if the container holds a result value.
    /// </summary>
    /// <param name="onResult">The action to execute.</param>
    public void Match(Action<T1> onResult, Action onNoResult, Action<List<IError>>? onErrors = null, Action<Exception>? onException = null)
    {
        switch (State)
        {
            case UnionContainerState.Result :
            {
                onResult(ResultValue.Item1);
                break;
            }
            default :
            {
                this.HandleNonResultStates(onNoResult, onErrors, onException);
                break;
            }
        }
    }


    /// <summary>
    /// Executes a function if the container holds a result value and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onResult">The function to execute.</param>
    /// <returns>The result of the function or the default value of <typeparamref name="TResult"/>.</returns>
    public TResult Match<TResult>(Func<T1, TResult> onResult, Func<TResult> onNoResult, Func<List<IError>,TResult>? onErrors = null, Func<Exception,TResult>? onException = null)
    {
        switch (State)
        {
            case UnionContainerState.Result : 
                return onResult(ResultValue.Item1);
            default : 
                return this.HandleNonResultStates(onNoResult, onErrors, onException);
        }
    }
    
    
    //conversion operators & constructors & deconstruction
    public static implicit operator UnionContainer<T1>(T1? value)           => new(value);
    public static implicit operator UnionContainer<T1>(Exception ex)        => new(ex);
    public static implicit operator UnionContainer<T1>(List<IError> errors) => new(errors.ToArray());
}