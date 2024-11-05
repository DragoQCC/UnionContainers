using HelpfulTypesAndExtensions;

namespace UnionContainers;

public record struct UnionContainer<T1, T2> : IUnionContainerResult<ValueTuple<T1, T2>>
{
    internal UnionContainerState State { get; set; }
    /// <inheritdoc/>
    UnionContainerState IUnionContainer.State
    {
        get => State;
        set => State = value;
    }
    
    internal List<IError>? Errors { get; set; }
    /// <inheritdoc/>
    List<IError>? IUnionContainer.Errors
    {
        get => Errors;
        set => Errors = value;
    }

    internal ValueTuple<T1, T2> ResultValue { get; init; }
    /// <inheritdoc/>
    ValueTuple<T1, T2> IUnionContainerResult<ValueTuple<T1, T2>>.ResultValue
    {
        get => ResultValue;
        init => ResultValue = value;
    }
    
    
    public UnionContainer()
    {
    }

    public UnionContainer(T1 value)
    {
        if (value is not null)
        {
            ResultValue = new ValueTuple<T1, T2>(value, default(T2)!);
            State = UnionContainerState.Result;
        }
    }

    public UnionContainer(T2 value)
    {
        if (value is not null)
        {
            ResultValue = new ValueTuple<T1, T2>(default(T1)!, value);
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
    public UnionContainer(Exception ex) : this(CustomErrors.Exception(ex))
    {
        State = UnionContainerState.Exception;
    }
    
    
    // deconstruction method
    public void Deconstruct(out T1? value1, out T2? value2)
    {
        value1 = default(T1);
        value2 = default(T2);

        switch (ResultValue)
        {
            case T1 t1 :
                value1 = t1;
                break;
            case T2 t2 :
                value2 = t2;
                break;
        }
    }
    
    public void Match(Action<T1> onT1Result, Action<T2> onT2Result, Action onNoResult, Action<List<IError>>? onErrors = null, Action<Exception>? onException = null)
    {
        if(State == UnionContainerState.Result)
        {
            this.HandleResultState(onT1Result, onT2Result);
        }
        else
        {
            this.HandleNonResultStates(onNoResult, onErrors, onException);
        }
    }

    public TResult Match<TResult>(Func<T1, TResult> onT1Result, Func<T2, TResult> onT2Result, Func<TResult> onNoResult, Func<List<IError>,TResult>? onErrors = null, Func<Exception,TResult>? onException = null)
    {
        return State switch
        {
            UnionContainerState.Result => HandleResultState(onT1Result, onT2Result),
            _ => this.HandleNonResultStates(onNoResult, onErrors, onException)
        };
    }
    
    private void HandleResultState(Action<T1> onT1Result, Action<T2> onT2Result)
    {
        var (t1, t2) = ResultValue;
        if (t1.IsNotDefault())
        {
            onT1Result(t1);
        }
        else
        {
            onT2Result(t2);
        }
    }
    
    private TResult HandleResultState<TResult>(Func<T1, TResult> onT1Result, Func<T2, TResult> onT2Result)
    {
        var (t1, t2) = ResultValue;
        return t1.IsNotDefault() ? onT1Result(t1) : onT2Result(t2);
    }
    
    
    // Implicit conversion operators
    public static implicit operator UnionContainer<T1,T2>(T1? value)       => new(value);
    public static implicit operator UnionContainer<T1,T2>(T2? value)       => new(value);
    public static implicit operator UnionContainer<T1,T2>(Exception ex)    => new(ex);
    public static implicit operator UnionContainer<T1,T2>(List<IError> errors) => new(errors.ToArray());
}