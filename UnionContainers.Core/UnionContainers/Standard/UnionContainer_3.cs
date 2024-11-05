using HelpfulTypesAndExtensions;

namespace UnionContainers;

public record struct UnionContainer<T1,T2,T3> : IUnionContainerResult<ValueTuple<T1,T2,T3>>
{
    internal UnionContainerState State { get; set; }
    internal List<IError>? Errors { get; set; }
    internal (T1, T2, T3) ResultValue { get; init; }

    /// <inheritdoc />
    UnionContainerState IUnionContainer.State
    {
        get => State;
        set => State = value;
    }
    /// <inheritdoc />
    List<IError>? IUnionContainer.Errors
    {
        get => Errors;
        set => Errors = value;
    }
    /// <inheritdoc />
    (T1, T2, T3) IUnionContainerResult<(T1, T2, T3)>.ResultValue
    {
        get => ResultValue;
        init => ResultValue = value;
    }
    
    public UnionContainer()
    {
    }

    public UnionContainer(T1? value)
    {
        if (value is not null)
        {
            ResultValue = new ValueTuple<T1, T2,T3>(value, default(T2), default(T3));
            State = UnionContainerState.Result;
        }
    }

    public UnionContainer(T2? value)
    {
        if (value is not null)
        {
            ResultValue = new ValueTuple<T1,T2,T3>(default(T1), value, default(T3));
            State = UnionContainerState.Result;
        }
    }
    
    public UnionContainer(T3? value)
    {
        if (value is not null)
        {
            ResultValue = new ValueTuple<T1,T2,T3>(default(T1), default(T2), value);
            State = UnionContainerState.Result;
        }
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UnionContainer{T1,T2}"/> struct with a list of errors.
    /// </summary>
    /// <param name="error">The errors to add to the container.</param>
    public UnionContainer(params IError[] error)
    {
        foreach (IError e in error)
        {
            Errors ??= new List<IError>();
            Errors.Add(e);
            if(e is CustomErrors.ExceptionWrapperError)
            {
                State = UnionContainerState.Exception;
            }
            else
            {
                State = UnionContainerState.Error;
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnionContainer{T1,T2}"/> struct with an exception.
    /// </summary>
    /// <param name="ex">The exception to wrap in an error.</param>
    public UnionContainer(Exception ex) : this (CustomErrors.Exception(ex))
    { }
    
    // deconstruction method
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3)
    {
        value1 = default(T1);
        value2 = default(T2);
        value3 = default(T3);

        switch (ResultValue)
        {
            case T1 t1 :
                value1 = t1;
                break;
            case T2 t2 :
                value2 = t2;
                break;
            case T3 t3 :
                value3 = t3;
                break;
        }
    }
    
    public void Match(Action<T1> onT1Result, Action<T2> onT2Result, Action<T3> onT3Result, Action onNoResult, Action<List<IError>>? onErrors = null, Action<Exception>? onException = null)
    {
        switch (State)
        {
            case UnionContainerState.Result:
                HandleResultState(onT1Result, onT2Result, onT3Result);
                break;
            default:
            {
                this.HandleNonResultStates(onNoResult, onErrors, onException);
                break;
            }
        }
    }

    public TResult Match<TResult>(Func<T1, TResult> onT1Result, Func<T2, TResult> onT2Result, Func<T3,TResult> onT3Result, Func<TResult> onNoResult, Func<List<IError>,TResult>? onErrors = null, Func<Exception,TResult>? onException = null)
    {
        return State switch
        {
            UnionContainerState.Result => HandleResultState(onT1Result, onT2Result, onT3Result),
            _ => this.HandleNonResultStates(onNoResult, onErrors, onException)
        };
    }
    
    private void HandleResultState(Action<T1> onT1Result, Action<T2> onT2Result, Action<T3> onT3Result)
    {
        var (t1, t2, t3) = ResultValue;
        if (t1.IsNotDefault())
        {
            onT1Result(t1);
        }
        else if (t2.IsNotDefault())
        {
            onT2Result(t2);
        }
        else
        {
            onT3Result(t3);
        }
    }
    
    
    private TResult HandleResultState<TResult>(Func<T1, TResult> onT1Result, Func<T2, TResult> onT2Result, Func<T3, TResult> onT3Result)
    {
        var (t1, t2,t3) = ResultValue;
        return t1.IsNotDefault() 
            ? onT1Result(t1) : t2.IsNotDefault() 
            ? onT2Result(t2) : onT3Result(t3);
    }
    
    
    // Implicit conversion operators
    public static implicit operator UnionContainer<T1,T2,T3>(T1? value)       => new(value);
    public static implicit operator UnionContainer<T1,T2,T3>(T2? value)       => new(value);
    public static implicit operator UnionContainer<T1,T2,T3>(T3? value)       => new(value);
    public static implicit operator UnionContainer<T1,T2,T3>(Exception ex)    => new(ex);
    public static implicit operator UnionContainer<T1,T2,T3>(List<IError> errors) => new(errors.ToArray());
}