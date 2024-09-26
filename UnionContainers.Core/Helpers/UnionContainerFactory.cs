using System.Runtime.CompilerServices;
using HelpfulTypesAndExtensions;

namespace UnionContainers;

public class UnionContainerFactory
{
    public static TContainer Create<TContainer, TValue>()
    where TContainer : IUnionContainer, new()
        => new();


    public static TContainer Create<TContainer, TValue>(IError error)
    where TContainer : struct, IUnionContainer
    {
        TContainer container = new();
        container.AddError(error);
        return container;
    }

    public static TContainer Create<TContainer, TValue>(Exception exception)
    where TContainer : struct, IUnionContainer
    {
        TContainer container = new();
        container.AddError(CustomErrors.Exception(exception));
        return container;
    }

    public static TContainer Create<TContainer, TValue>(TValue result)
    where TContainer : struct, IUnionResultContainer<TValue>
    where TValue : struct, ITuple
    {
        var container = new TContainer();
        return container.TryCreateResult(result);
    }


    public static UnionContainer<TValue> Create<TValue>() => new();

    public static UnionContainer<TValue> Create<TValue>(IError error) => new(error);

    public static UnionContainer<TValue> Create<TValue>(Exception exception) => new(CustomErrors.Exception(exception));
    public static UnionContainer<TValue> Create<TValue>(TValue result)       => new(result);



    public static TContainer MethodToContainer<TContainer, T>(Action<T?> action, T? item)
    where TContainer : struct, IUnionContainer<TContainer>
    {
        try
        {
            action(item);
            return Create<TContainer, T>();
        }
        catch (Exception e)
        {
            return Create<TContainer, T>(e);
        }
    }
}