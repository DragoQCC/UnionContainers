using Microsoft.Extensions.Logging;
using UnionContainers.Core.Common;
using UnionContainers.Core.UnionContainers;
using UnionContainers.Shared.Common;

namespace UnionContainers.Core.Helpers;

public partial class UnionContainerFactory
{
    /// <summary>
    /// Produces a container with the error state set and the errors provided added to its error values
    /// </summary>
    /// <param name="errors"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static UnionContainer<T> CreateWithError<T>(params object[] errors) => new UnionContainer<T>().AddError(errors);
    
    /// <summary>
    /// Creates a container with the exception state set and the exception provided as the exception value
    /// </summary>
    /// <param name="ex"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static UnionContainer<T> CreateWithException<T>(Exception ex) => new UnionContainer<T>().SetException(ex);
    
    /// <summary>
    /// Set the value state of the container to the value provided
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static UnionContainer<T> CreateWithValue<T>(T value)
    {
         UnionContainer<T> container = new();
         return container.SetValue(value);
    }
    
    
    /// <summary>
    /// Wraps the supplied method in a try catch <br/>
    /// Sets the container value state to the methods result if the method executes successfully <br/>
    /// Sets the container to an exception state if the method throws an exception, includes the exception in the exception state <br/>
    /// Includes the ability to pass in a parameter to the method
    /// </summary>
    /// <param name="action"></param>
    /// <param name="item"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static TContainer MethodToContainer<TContainer,T>(Action<T?> action, T? item) where TContainer : UnionContainerBase<TContainer>, IUnionContainer<TContainer>
    {
        TContainer container = TContainer.Create();
        try
        {
            action(item);
            container.SetEmpty(Empty.Nothing);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    
    /// <summary>
    /// Wraps the supplied method in a try catch <br/>
    /// Sets the container value state to the methods result if the method executes successfully <br/>
    /// Sets the container to an exception state if the method throws an exception, includes the exception in the exception state <br/>
    /// </summary>
    /// <param name="method"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static TContainer MethodToContainer<TContainer,TResult>(Func<TResult> method) where TContainer : UnionContainerBase<TContainer>, IUnionContainer<TContainer>, IResultTypeWrapper<TResult,IResult>
    {
        TContainer container = TContainer.Create();
        try
        {
            TResult? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1> MethodToContainer<TResult1>(Func<object> method)
    {
        UnionContainer<TResult1> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2> MethodToContainer<TResult1,TResult2>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3> MethodToContainer<TResult1,TResult2,TResult3>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3,TResult4> MethodToContainer<TResult1,TResult2,TResult3,TResult4>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3,TResult4> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5> MethodToContainer<TResult1,TResult2,TResult3,TResult4,TResult5>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6> MethodToContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7> MethodToContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8> MethodToContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9> MethodToContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10> MethodToContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11> MethodToContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12> MethodToContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12,TResult13> MethodToContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12,TResult13>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12,TResult13> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12,TResult13,TResult14> MethodToContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12,TResult13,TResult14>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12,TResult13,TResult14> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12,TResult13,TResult14,TResult15> MethodToContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12,TResult13,TResult14,TResult15>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12,TResult13,TResult14,TResult15> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12,TResult13,TResult14,TResult15,TResult16> MethodToContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12,TResult13,TResult14,TResult15,TResult16>(Func<object> method)
    {
        UnionContainer<TResult1,TResult2,TResult3,TResult4,TResult5,TResult6,TResult7,TResult8,TResult9,TResult10,TResult11,TResult12,TResult13,TResult14,TResult15,TResult16> container = new();
        try
        {
            object? resultItem = method() ?? default;
            container.SetValue(resultItem);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    
    
    public static async Task<TContainer> MethodToContainer<TContainer>(Func<Task> method) where TContainer : UnionContainerBase<TContainer>, IUnionContainer<TContainer>
    {
        TContainer container = TContainer.Create();
        try
        {
            await method();
            container.SetEmpty(Empty.Nothing);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static async Task<UnionContainer<TResult>> MethodToContainer<TResult>(Func<Task<TResult>> action)
    {
        UnionContainer<TResult> container = new();
        try
        {
            var taskResult = await action();
            container.SetValue(taskResult);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    
    
    public static async Task<UnionContainer<T1,T2>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2>(TMethod method)
    {
        UnionContainer<T1,T2> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => result = func(),
                Task<object> task => result = await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
    public static async Task<UnionContainer<T1,T2,T3>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3>(TMethod method)
    {
        UnionContainer<T1,T2,T3> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => result = func(),
                Task<object> task => result = await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }

    public static async Task<UnionContainer<T1,T2,T3,T4>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3,T4>(TMethod method)
    {
        UnionContainer<T1,T2,T3,T4> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => func(),
                Task<object> task => await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }

    public static async Task<UnionContainer<T1,T2,T3,T4,T5>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3,T4,T5>(TMethod method)
    {
        UnionContainer<T1,T2,T3,T4,T5> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => func(),
                Task<object> task => await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }

    public static async Task<UnionContainer<T1,T2,T3,T4,T5,T6>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3,T4,T5,T6>(TMethod method)
    {
        UnionContainer<T1,T2,T3,T4,T5,T6> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => func(),
                Task<object> task => await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }

    public static async Task<UnionContainer<T1,T2,T3,T4,T5,T6,T7>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3,T4,T5,T6,T7>(TMethod method)
    {
        UnionContainer<T1,T2,T3,T4,T5,T6,T7> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => func(),
                Task<object> task => await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }

    public static async Task<UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3,T4,T5,T6,T7,T8>(TMethod method)
    {
        UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => func(),
                Task<object> task => await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }

    public static async Task<UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3,T4,T5,T6,T7,T8,T9>(TMethod method)
    {
        UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => func(),
                Task<object> task => await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }

    public static async Task<UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>(TMethod method)
    {
        UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => func(),
                Task<object> task => await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }

    public static async Task<UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11>(TMethod method)
    {
        UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => func(),
                Task<object> task => await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }

    public static async Task<UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12>(TMethod method)
    {
        UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => func(),
                Task<object> task => await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }

    public static async Task<UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13>(TMethod method)
    {
        UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => func(),
                Task<object> task => await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }

    public static async Task<UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14>(TMethod method)
    {
        UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => func(),
                Task<object> task => await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }

    public static async Task<UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15>(TMethod method)
    {
        UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => func(),
                Task<object> task => await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }

    public static async Task<UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>> MethodToContainer<[AllowedTypes<Task<object>,Func<object>,Action>]TMethod,T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>(TMethod method)
    {
        UnionContainer<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16> container = new();
        try
        {
            object? result = method switch
            {
                Action action => action.TryCatch(),
                Func<object> func => func(),
                Task<object> task => await task,
                _ => container.SetException(new InvalidOperationException())
            };
            container.SetValue(result);
        }
        catch (Exception e)
        {
            container.LogMethodToContainerException(e);
        }
        return container;
    }
    
}