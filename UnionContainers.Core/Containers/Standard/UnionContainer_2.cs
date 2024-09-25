using HelpfulTypesAndExtensions;

namespace UnionContainers;

public record struct UnionContainer<T1,T2> : IUnionResultContainer<ValueTuple<T1,T2>>
{
    
    
    internal UnionContainerState State { get; set; }
    /// <inheritdoc />
    UnionContainerState IUnionContainer.State
    {
        get => State;
        set => State = value;
    }

    /// <inheritdoc />
    public Exception? ExceptionValue { get; set; }


    /// <inheritdoc />
    public List<IError>? Errors { get; set; }


    internal ValueTuple<T1,T2> ResultValue { get; init; }


    /// <inheritdoc />
    ValueTuple<T1,T2> IUnionResultContainer<ValueTuple<T1,T2>>.ResultValue
    {
        get => ResultValue;
        init => ResultValue = value;
    }
    
    
    // deconstruction method
    public void Deconstruct(out T1? value1, out T2? value2)
    {
        value1 = default;
        value2 = default;

        switch (ResultValue)
        {
            case T1 t1:
                value1 = t1;
                break;
            case T2 t2:
                value2 = t2;
                break;
        }
    }

    /// <inheritdoc />
    public void Match()
    {
        
    }
    
    /// <inheritdoc />
    public TResult Match<TResult>() => default;
    
    
    public UnionContainer()
    {}
    public UnionContainer(T1 value)
    {
        if (value is not null)
        {
            ResultValue = new(value, default);
            State = UnionContainerState.Result;
        }
    }
    
    public UnionContainer(T2 value)
    {
        if (value is not null)
        {
            ResultValue = new(default,value);
            State = UnionContainerState.Result;
        }
    }
    
    // Implicit conversion operators
    public static implicit operator UnionContainer<T1, T2>(T1? value) => new UnionContainer<T1, T2>(value);
    public static implicit operator UnionContainer<T1, T2>(T2? value) => new UnionContainer<T1, T2>(value);

}