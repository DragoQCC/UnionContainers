
using HelpfulTypesAndExtensions;

namespace UnionContainers;

public record struct UnionContainer<T1, T2, T3, T4, T5, T6, T7> : IUnionContainerResult<(T1, T2, T3, T4, T5, T6, T7)>
{
    internal UnionContainerState State { get; set; }
    internal List<IError>? Errors { get; set; }
    internal (T1, T2, T3, T4, T5, T6, T7) ResultValue { get; init; }

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
    (T1, T2, T3, T4, T5, T6, T7) IUnionContainerResult<(T1, T2, T3, T4, T5, T6, T7)>.ResultValue
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
            ResultValue = (value, default(T2), default(T3), default(T4), default(T5), default(T6), default(T7));
            State = UnionContainerState.Result;
        }
    }

    public UnionContainer(T2? value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), value, default(T3), default(T4), default(T5), default(T6), default(T7));
            State = UnionContainerState.Result;
        }
    }

    public UnionContainer(T3? value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), default(T2), value, default(T4), default(T5), default(T6), default(T7));
            State = UnionContainerState.Result;
        }
    }

    public UnionContainer(T4? value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), default(T2), default(T3), value, default(T5), default(T6), default(T7));
            State = UnionContainerState.Result;
        }
    }

    public UnionContainer(T5? value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), default(T2), default(T3), default(T4), value, default(T6), default(T7));
            State = UnionContainerState.Result;
        }
    }
    
    public UnionContainer(T6? value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), default(T2), default(T3), default(T4), default(T5), value, default(T7));
            State = UnionContainerState.Result;
        }
    }
    
    public UnionContainer(T7? value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), value);
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
    {
    }
    
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3, out T4? value4, out T5? value5, out T6? value6, out T7? value7)
    {
        value1 = default;
        value2 = default;
        value3 = default;
        value4 = default;
        value5 = default;
        value6 = default;
        value7 = default;

        switch (ResultValue)
        {
            case T1 t1:
                value1 = t1;
                break;
            case T2 t2:
                value2 = t2;
                break;
            case T3 t3:
                value3 = t3;
                break;
            case T4 t4:
                value4 = t4;
                break;
            case T5 t5:
                value5 = t5;
                break;
            case T6 t6:
                value6 = t6;
                break;
            case T7 t7:
                value7 = t7;
                break;
        }
    }
    
    public void Match(Action<T1> onT1Result, Action<T2> onT2Result, Action<T3> onT3Result, Action<T4> onT4Result, Action<T5> onT5Result, Action<T6> onT6Result, Action<T7> onT7Result, Action onNoResult, Action<List<IError>>? onErrors = null, Action<Exception>? onException = null)
    {
        if(State == UnionContainerState.Result)
        {
            this.HandleResultState(onT1Result, onT2Result, onT3Result, onT4Result, onT5Result, onT6Result, onT7Result);
        }
        else
        {
            this.HandleNonResultStates(onNoResult, onErrors, onException);
        }
    }
    
    public TResult Match<TResult>(Func<T1, TResult> onT1Result, Func<T2, TResult> onT2Result, Func<T3, TResult> onT3Result, Func<T4, TResult> onT4Result, Func<T5, TResult> onT5Result, Func<T6, TResult> onT6Result, Func<T7, TResult> onT7Result, Func<TResult> onNoResult, Func<List<IError>,TResult>? onErrors = null, Func<Exception,TResult>? onException = null)
    {
        return State switch
        {
            UnionContainerState.Result => this.HandleResultState(onT1Result, onT2Result, onT3Result, onT4Result, onT5Result, onT6Result, onT7Result),
            _ => this.HandleNonResultStates(onNoResult, onErrors, onException)
        };
    }
    
    private void HandleResultState(Action<T1> onT1Result, Action<T2> onT2Result, Action<T3> onT3Result, Action<T4> onT4Result, Action<T5> onT5Result, Action<T6> onT6Result, Action<T7> onT7Result)
    {
        var (t1, t2, t3, t4, t5, t6, t7) = ResultValue;
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
        else if (t4.IsNotDefault())
        {
            onT4Result(t4);
        }
        else if (t5.IsNotDefault())
        {
            onT5Result(t5);
        }
        else if (t6.IsNotDefault())
        {
            onT6Result(t6);
        }
        else
        {
            onT7Result(t7);
        }
    }
    
    private TResult HandleResultState<TResult>(Func<T1, TResult> onT1Result, Func<T2, TResult> onT2Result, Func<T3, TResult> onT3Result, Func<T4, TResult> onT4Result, Func<T5, TResult> onT5Result, Func<T6, TResult> onT6Result, Func<T7, TResult> onT7Result)
    {
        var (t1, t2, t3, t4, t5, t6, t7) = ResultValue;
        return t1.IsNotDefault() ? onT1Result(t1) 
            : t2.IsNotDefault() ? onT2Result(t2) 
            : t3.IsNotDefault() ? onT3Result(t3) 
            : t4.IsNotDefault() ? onT4Result(t4) 
            : t5.IsNotDefault() ? onT5Result(t5) 
            : t6.IsNotDefault() ? onT6Result(t6) 
            : onT7Result(t7);
    }
    
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T1? value) => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T2? value) => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T3? value) => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T4? value) => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T5? value) => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T6? value) => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(T7? value)     => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6, T7>(Exception ex) => new(ex);
    public static implicit operator UnionContainer<T1,T2,T3,T4,T5, T6, T7>(List<IError> errors) => new(errors.ToArray());
}