using HelpfulTypesAndExtensions;

namespace UnionContainers;

public record struct UnionContainer<T1> : IUnionResultContainer<ValueTuple<T1>>
{
    public UnionContainer(T1? value)
    {
        if (value is not null)
        {
            ResultValue = new ValueTuple<T1>(value);
            State = UnionContainerState.Result;
        }
    }

    public UnionContainer(params IError[] error)
    {
        foreach (IError e in error)
        {
            Errors ??= new List<IError>();
            Errors.Add(e);
        }

        State = UnionContainerState.Error;
    }

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
    ValueTuple<T1> IUnionResultContainer<ValueTuple<T1>>.ResultValue
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


    public static UnionContainer<T1> Empty() => new();

    public static UnionContainer<T1> Error<TError>(TError error)
    where TError : struct, IError
    {
        UnionContainer<T1> container = new();
        container.AddError(error);
        return container;
    }

    public static UnionContainer<T1> Result(T1 value) => new(value);





    public void Match(Action<T1> action)
    {
        if (State == UnionContainerState.Result)
        {
            action(ResultValue.Item1);
        }
    }


    public TResult? Match<TResult>(Func<T1, TResult> method)
    {
        if (State == UnionContainerState.Result)
        {
            return method(ResultValue.Item1);
        }

        return default(TResult?);
    }

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


    //conversion operators & constructors & deconstruction
    public static implicit operator UnionContainer<T1>(T1? value)           => new(value);
    public static implicit operator UnionContainer<T1>(Exception ex)        => new(CustomErrors.Exception(ex));
    public static implicit operator UnionContainer<T1>(List<IError> errors) => new(errors.ToArray());
}