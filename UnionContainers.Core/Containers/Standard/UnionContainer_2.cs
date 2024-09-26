using HelpfulTypesAndExtensions;

namespace UnionContainers;

public record struct UnionContainer<T1, T2> : IUnionResultContainer<ValueTuple<T1, T2>>
{
    public UnionContainer()
    {
    }

    public UnionContainer(T1 value)
    {
        if (value is not null)
        {
            ResultValue = new ValueTuple<T1, T2>(value, default(T2));
            State = UnionContainerState.Result;
        }
    }

    public UnionContainer(T2 value)
    {
        if (value is not null)
        {
            ResultValue = new ValueTuple<T1, T2>(default(T1), value);
            State = UnionContainerState.Result;
        }
    }


    internal UnionContainerState State { get; set; }

    /// <inheritdoc/>
    public Exception? ExceptionValue { get; set; }


    internal ValueTuple<T1, T2> ResultValue { get; init; }

    /// <inheritdoc/>
    UnionContainerState IUnionContainer.State
    {
        get => State;
        set => State = value;
    }


    /// <inheritdoc/>
    public List<IError>? Errors { get; set; }


    /// <inheritdoc/>
    ValueTuple<T1, T2> IUnionResultContainer<ValueTuple<T1, T2>>.ResultValue
    {
        get => ResultValue;
        init => ResultValue = value;
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

    /// <inheritdoc/>
    public void Match()
    {
    }

    /// <inheritdoc/>
    public TResult Match<TResult>() => default;

    // Implicit conversion operators
    public static implicit operator UnionContainer<T1, T2>(T1? value) => new(value);
    public static implicit operator UnionContainer<T1, T2>(T2? value) => new(value);
}