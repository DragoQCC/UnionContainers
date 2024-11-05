using System.Linq.Expressions;
using System.Security.Cryptography;
using HelpfulTypesAndExtensions;

namespace UnionContainers;

public record struct UnionContainer<T1, T2, T3, T4, T5, T6> : IUnionContainerResult<(T1, T2, T3, T4, T5, T6)>
{
    internal UnionContainerState State { get; set; }
    internal List<IError>? Errors { get; set; }
    internal (T1, T2, T3, T4, T5, T6) ResultValue { get; init; }

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
    (T1, T2, T3, T4, T5, T6) IUnionContainerResult<(T1, T2, T3, T4, T5, T6)>.ResultValue
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
            ResultValue = (value, default(T2), default(T3), default(T4), default(T5), default(T6));
            State = UnionContainerState.Result;
        }
    }

    public UnionContainer(T2? value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), value, default(T3), default(T4), default(T5), default(T6));
            State = UnionContainerState.Result;
        }
    }
    
    public UnionContainer(T3? value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), default(T2), value, default(T4), default(T5), default(T6));
            State = UnionContainerState.Result;
        }
    }
    
    public UnionContainer(T4? value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), default(T2), default(T3), value, default(T5), default(T6));
            State = UnionContainerState.Result;
        }
    }
    
    public UnionContainer(T5? value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), default(T2), default(T3), default(T4), value, default(T6));
            State = UnionContainerState.Result;
        }
    }
    
    public UnionContainer(T6? value)
    {
        if (value is not null)
        {
            ResultValue = (default(T1), default(T2), default(T3), default(T4), default(T5), value);
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
    
    public void Deconstruct(out T1? value1, out T2? value2, out T3? value3, out T4? value4, out T5? value5, out T6? value6)
    {
        value1 = default;
        value2 = default;
        value3 = default;
        value4 = default;
        value5 = default;
        value6 = default;

        switch (State)
        {
            case UnionContainerState.Result:
                value1 = ResultValue.Item1;
                value2 = ResultValue.Item2;
                value3 = ResultValue.Item3;
                value4 = ResultValue.Item4;
                value5 = ResultValue.Item5;
                value6 = ResultValue.Item6;
                break;
        }
    }
    
    public void Match(Action<T1> onResult1, Action<T2> onResult2, Action<T3> onResult3, Action<T4> onResult4, Action<T5> onResult5, Action<T6> onResult6, Action onNoResult, Action<List<IError>>? onErrors = null, Action<Exception>? onException = null)
    {
        switch (State)
        {
            case UnionContainerState.Result:
            {
                this.HandleResultState(onResult1, onResult2, onResult3, onResult4, onResult5, onResult6);
                break;
            }
            default:
            {
                this.HandleNonResultStates(onNoResult, onErrors, onException);
                break;
            }
        }
    }
    
    public TResult Match<TResult>(Func<T1, TResult> onResult1, Func<T2, TResult> onResult2, Func<T3, TResult> onResult3, Func<T4, TResult> onResult4, Func<T5, TResult> onResult5, Func<T6, TResult> onResult6, Func<TResult> onNoResult, Func<List<IError>,TResult>? onErrors = null, Func<Exception,TResult>? onException = null)
    {
        return State switch
        {
            UnionContainerState.Result => this.HandleResultState(onResult1, onResult2, onResult3, onResult4, onResult5, onResult6),
            _ => this.HandleNonResultStates(onNoResult, onErrors, onException)
        };
    }
    
    private void HandleResultState(Action<T1> onT1Result, Action<T2> onT2Result, Action<T3> onT3Result, Action<T4> onT4Result, Action<T5> onT5Result, Action<T6> onT6Result)
    {
        var (t1, t2, t3, t4, t5, t6) = ResultValue;
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
    }
    
    private TResult HandleResultState<TResult>(Func<T1, TResult> onT1Result, Func<T2, TResult> onT2Result, Func<T3, TResult> onT3Result, Func<T4, TResult> onT4Result, Func<T5, TResult> onT5Result, Func<T6, TResult> onT6Result)
    {
        var (t1, t2, t3, t4, t5, t6) = ResultValue;
        return t1.IsNotDefault() ? onT1Result(t1) 
            : t2.IsNotDefault() ? onT2Result(t2) 
            : t3.IsNotDefault() ? onT3Result(t3) 
            : t4.IsNotDefault() ? onT4Result(t4) 
            : t5.IsNotDefault() ? onT5Result(t5) 
                                : onT6Result(t6);
    }
    
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6>(T1? value)       => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6>(T2? value)       => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6>(T3? value)       => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6>(T4? value)       => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6>(T5? value)       => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6>(T6? value)       => new(value);
    public static implicit operator UnionContainer<T1, T2, T3, T4, T5, T6>(Exception ex)    => new(ex);
    public static implicit operator UnionContainer<T1,T2,T3,T4,T5, T6>(List<IError> errors) => new(errors.ToArray());
    
}

