
using HelpfulTypesAndExtensions;

namespace UnionContainers;

public record struct UnionContainer<T1, T2, T3, T4> : IUnionContainerResult<(T1, T2, T3, T4)>
{
    internal UnionContainerState State { get; set; }
    internal List<IError>? Errors { get; set; }
    internal (T1, T2, T3, T4) ResultValue { get; init; }

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
    (T1, T2, T3, T4) IUnionContainerResult<(T1, T2, T3, T4)>.ResultValue
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
            ResultValue = (value, default(T2), default(T3), default(T4));
            State = UnionContainerState.Result;
        }
    }

    public UnionContainer(T2 value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), value, default(T3), default(T4));
            State = UnionContainerState.Result;
        }
    }
    
    public UnionContainer(T3 value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), default(T2), value, default(T4));
            State = UnionContainerState.Result;
        }
    }
    
    public UnionContainer(T4 value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), default(T2), default(T3), value);
            State = UnionContainerState.Result;
        }
    }
    
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
    
    public UnionContainer(Exception ex) : this(CustomErrors.Exception(ex))
    {}
    
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3, out T4? value4)
    {
        value1 = default;
        value2 = default;
        value3 = default;
        value4 = default;

        switch (State)
        {
            case UnionContainerState.Result:
                value1 = ResultValue.Item1;
                value2 = ResultValue.Item2;
                value3 = ResultValue.Item3;
                value4 = ResultValue.Item4;
                break;
        }
    }
    
    public void Match(Action<T1> onT1Result, Action<T2> onT2Result, Action<T3> onT3Result, Action<T4> onT4Result, Action onNoResult, Action<List<IError>>? onErrors = null, Action<Exception>? onException = null)
    {
        switch (State)
        {
            case UnionContainerState.Result:
                HandleResultState(onT1Result, onT2Result, onT3Result, onT4Result);
                break;
            default:
            {
                this.HandleNonResultStates(onNoResult, onErrors, onException);
                break;
            }
        }
    }
    
    public TResult Match<TResult>(Func<T1, TResult> onT1Result, Func<T2, TResult> onT2Result, Func<T3, TResult> onT3Result, Func<T4, TResult> onT4Result, Func<TResult> onNoResult, Func<List<IError>,TResult>? onErrors = null, Func<Exception,TResult>? onException = null)
    {
        return State switch
        {
            UnionContainerState.Result =>  HandleResultState(onT1Result, onT2Result, onT3Result, onT4Result),
            _ => this.HandleNonResultStates(onNoResult, onErrors, onException)
        };
    }
    
    private void HandleResultState(Action<T1> onT1Result, Action<T2> onT2Result, Action<T3> onT3Result, Action<T4> onT4Result)
    {
        var (t1, t2, t3, t4) = ResultValue;
        if (t1.IsNotDefault())
        {
            onT1Result(t1);
        }
        else if (t2.IsNotDefault())
        {
            onT2Result(t2);
        }
        else if (t3.IsNotDefault())
        {
            onT3Result(t3);
        }
        else
        {
            onT4Result(t4);
        }
    }
    
    
    private TResult HandleResultState<TResult>(Func<T1, TResult> onT1Result, Func<T2, TResult> onT2Result, Func<T3, TResult> onT3Result, Func<T4, TResult> onT4Result)
    {
        var (t1, t2, t3, t4) = ResultValue;
        return t1.IsNotDefault() ? onT1Result(t1) 
            : t2.IsNotDefault()  ? onT2Result(t2) 
            : t3.IsNotDefault()  ? onT3Result(t3) 
            : onT4Result(t4);
    }
    
    
    public static implicit operator UnionContainer<T1, T2, T3, T4>(T1? value) => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4>(T2? value) => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4>(T3? value) => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4>(T4? value) => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4>(List<IError> errors) => new(errors.ToArray());
    public static implicit operator UnionContainer<T1, T2, T3, T4>(Exception? ex) => new(ex);
    
}

