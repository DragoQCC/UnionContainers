using System.Runtime.CompilerServices;
using HelpfulTypesAndExtensions;
using Microsoft.Extensions.Logging;

namespace UnionContainers;

public static class UnionContainerExtensions
{
#region Conversion Extensions
    //Conversion Extensions
    
    /// <summary>
    /// Converts the object to a UnionContainer{T} <br/>
    /// Allows for safely wrapping a null object, an object, an error, or an exception in a UnionContainer container
    /// </summary>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static UnionContainer<T> ToContainer<T>(this T? item)
        => item is null
            ? UnionContainer<T>.EmptyContainer()
            : new UnionContainer<T>(item);


    /// <summary>
    /// Converts a nullable value type to a UnionContainer{T}
    /// If the value is null, returns an empty container.
    /// Otherwise, returns a container holding the value.
    /// </summary>
    /// <param name="item">The nullable value to be converted.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>A UnionContainer holding the value or an empty container if the value is null.</returns>
    public static UnionContainer<T> FromNullableToContainer<T>(this T? item)
    where T : struct
    {
        if (item is null)
        {
            return UnionContainer<T>.EmptyContainer();
        }
        return new UnionContainer<T>(item.Value);
    }
    
#endregion

#region State Checking Extensions
    //State Checking Extensions

    /// <summary>
    /// returns true if the container has any errors or exceptions set
    /// </summary>
    /// <param name="container"> </param>
    /// <typeparam name="TContainer"> </typeparam>
    /// <returns> </returns>
    public static bool IsMissingResult<TContainer>(this TContainer container)
    where TContainer : struct, IUnionContainer
        => container.HasResult() is false;


    /// <summary>
    ///     Returns `true` if the container is empty, `false` if it has a value
    ///     Also returns false when <see cref="UnionContainerOptions.ContainersNotEmptyIfIssues"/> is set to true and the container has issues (i.e. an error or exception)
    /// </summary>
    /// <param name="container"> </param>
    /// <typeparam name="TContainer"> </typeparam>
    /// <returns> </returns>
    public static bool IsEmpty<TContainer>(this TContainer container)
    where TContainer : struct, IUnionContainer
        =>  container.State is UnionContainerState.Empty;

    /// <summary>
    /// Returns `true` if the container has an error, `false` if it does not
    /// </summary>
    /// <param name="container"> </param>
    /// <typeparam name="TContainer"> </typeparam>
    /// <returns> </returns>
    public static bool HasErrors<TContainer>(this TContainer container)
    where TContainer : struct, IUnionContainer
        => container.State is UnionContainerState.Error;


    /// <summary>
    ///     Returns true if the container has a value set, false otherwise
    /// </summary>
    /// <param name="container"> </param>
    /// <typeparam name="TContainer"> </typeparam>
    /// <returns> </returns>
    public static bool HasResult<TContainer>(this TContainer container)
    where TContainer : struct, IUnionContainer
        => container.State is UnionContainerState.Result;


    /// <summary>
    /// Retrieves the state of the specified container.
    /// </summary>
    /// <typeparam name="TContainer">Type of the container</typeparam>
    /// <param name="container">The container to retrieve the state from</param>
    /// <returns>The state of the container</returns>
    public static UnionContainerState GetState<TContainer>(this TContainer container)
    where TContainer : IUnionContainer
        => container.State;
#endregion

#region State Handling Extensions

    /// <summary>
    ///     Executes the supplied action if the container is still empty <br/>
    ///     Container is empty if it has no value, error or exception set <br/>
    ///     Throws an exception if the action throws an exception and <see cref="UnionContainerOptions.ThrowExceptionsFromUserHandlingCode"/> is set to true <br/>
    /// </summary>
    /// <param name="container"> </param>
    /// <param name="action"> </param>
    /// <typeparam name="TContainer"> </typeparam>
    /// <returns> The original container instance so method calls can be chained </returns>
    public static TContainer IfEmptyDo<TContainer>(this ref TContainer container, Action action)
    where TContainer : struct, IUnionContainer
    {
        if (container.IsEmpty())
        {
            container.IfNotNullDo(action).ContinueWith(container);
        }
        return container;
    }


    /// <summary>
    ///     Executes the supplied action if the container has an exception set
    ///     Throws an exception if the action throws an exception and <see cref="UnionContainerOptions.ThrowExceptionsFromUserHandlingCode"/> is set to true <br/>
    /// </summary>
    /// <param name="container"> </param>
    /// <param name="action"> </param>
    /// <typeparam name="TContainer"> </typeparam>
    /// <returns> </returns>
    public static TContainer IfExceptionDo<TContainer>(this ref TContainer container, Action<Exception> action)
    where TContainer : struct, IUnionContainer
    {
        Exception? ex = container.GetException();
        if (ex is not null)
        {
            action(ex);
        }
        return container;
    }
    
    /// <summary>
    /// Executes the supplied action if the container has an error set
    /// </summary>
    /// <param name="container"></param>
    /// <param name="action"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static TContainer IfErrorDo<TContainer>(this ref TContainer container, Action<IError> action)
    where TContainer : struct, IUnionContainer
    {
        if (container.HasErrors())
        {
            action(container.GetErrors().First());
        }
        return container;
    }
    
    /// <summary>
    /// Executes the supplied action if the container is in the specified state
    /// </summary>
    /// <param name="container">The UnionContainer to perform the state check against</param>
    /// <param name="state"> The desired state to check for</param>
    /// <param name="action">The action to run if the container is in the desired state</param>
    /// <typeparam name="TContainer"></typeparam>
    public static void ForState<TContainer>(this TContainer container, UnionContainerState state, Action action)
    where TContainer : struct, IUnionContainer
        => container.ForState(state,action);

    /// <summary>
    /// Executes the supplied action if the container is in the specified state, can also take a default action to run if the container is not in the specified state
    /// </summary>
    /// <param name="container">The UnionContainer to perform the state check against</param>
    /// <param name="defaultAction">A fallback default action to run if the container is in a state not covered by one of the passed in targetState items</param>
    /// <param name="targetState"> Collection of ValueTuple pairs for the desired state to check for & the action to run if the container is in the desired state</param>
    /// <typeparam name="TContainer"></typeparam>
    public static void ForState<TContainer>(this TContainer container, Action? defaultAction = null, params ValueTuple<UnionContainerState, Action>[] targetState)
    where TContainer : struct, IUnionContainer
        => container.ForState(defaultAction, targetState);

    
    /// <summary>
    /// Executes the supplied action if the container is in the specified state, can also take a default action to run if the container is not in the specified state
    /// </summary>
    /// <param name="container"></param>
    /// <param name="onNoResult"></param>
    /// <param name="onErrors"></param>
    /// <param name="onException"></param>
    /// <typeparam name="TContainer"></typeparam>
    internal static void HandleNonResultStates<TContainer>(this TContainer container, Action onNoResult, Action<List<IError>?>? onErrors = null, Action<Exception?>? onException = null)
    where TContainer : struct, IUnionContainer
    {
        switch (container.State)
        {
            case UnionContainerState.Empty:
            {
                onNoResult();
                break;
            }
            case UnionContainerState.Error when onErrors is null:
            {
                foreach (IError error in container.GetErrors())
                {
                    UnionContainerOptions.InternalErrorService.TryHandleError(error);
                }
                break;
            }
            case UnionContainerState.Error when onErrors is not null:
            {
                onErrors?.Invoke(container.GetErrors());
                break;
            }
            case UnionContainerState.Exception when onException is not null:
            {
                onException?.Invoke(container.GetException());
                break;
            }
            default:
            {
                onNoResult();
                break;
            }
        }
    }

    /// <summary>
    /// Executes the supplied function if the container is in the specified state, can also take a default function to run if the container is not in the specified state
    /// </summary>
    /// <param name="container"></param>
    /// <param name="onNoResult"></param>
    /// <param name="onErrors"></param>
    /// <param name="onException"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TResult"> The type to return from the container</typeparam>
    /// <returns>An instance of the TResult type specified by calling the Match API on UnionContainers</returns>
    internal static TResult HandleNonResultStates<TContainer,TResult>(this TContainer container, Func<TResult> onNoResult, Func<List<IError>?,TResult>? onErrors = null, Func<Exception?,TResult>? onException = null)
    where TContainer : struct, IUnionContainer
    {
        return container.State switch
        {
            UnionContainerState.Empty => onNoResult(),
            UnionContainerState.Error when onErrors is not null => onErrors(container.GetErrors()),
            UnionContainerState.Exception when onException is not null => onException(container.GetException()),
            _ => onNoResult()
        };
    }
    
#endregion


#region Data Access Extensions
    
    /// <summary>
    /// Returns the exception of the container if one is set, otherwise returns null
    /// </summary>
    /// <param name="container"> </param>
    /// <typeparam name="TContainer"> </typeparam>
    /// <returns> </returns>
    public static Exception? GetException<TContainer>(this TContainer container)
    where TContainer : IUnionContainer
        => container.State switch
        {
            UnionContainerState.Error => container.GetErrors<CustomErrors.ExceptionWrapperError>().FirstOrDefault().exception.ReturnIf(x => x.IsNotDefault()),
            _ => null
        };

    /// <summary>
    /// Returns a list of errors as type <see cref="IError"/> from the container if any are set, otherwise returns an empty list
    /// </summary>
    /// <param name="container"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static List<IError> GetErrors<TContainer>(this TContainer container)
    where TContainer : IUnionContainer
        => container.GetErrors();

    
    /// <summary>
    /// Returns a list of errors as type <typeparamref name="TError"/> from the container if any are set, otherwise returns an empty list
    /// </summary>
    /// <param name="container"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static List<TError> GetErrors<TError>(this IUnionContainer container)
    where TError : struct, IError
        => container.GetErrors<TError>();

#endregion


#region DataModification
    
    /// <summary>
    /// Adds an error to the container
    /// </summary>
    /// <param name="container"></param>
    /// <param name="error"></param>
    /// <typeparam name="TContainer"></typeparam>
    public static void AddError<TContainer>(this ref TContainer container, IError error)
    where TContainer : struct, IUnionContainer
    {
        container.Errors ??= [ ];
        container.Errors.Add(error);
        if (container.State is UnionContainerState.Error)
        {
            return;
        }
        container.State = error is CustomErrors.ExceptionWrapperError
            ? UnionContainerState.Exception
            : UnionContainerState.Error;
    }

    
    /// <summary>
    /// Adds an error of type <see cref="TError"/> to the container
    /// </summary>
    /// <param name="container"></param>
    /// <param name="error"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TError">The specific type of <see cref="IError"/> to store in the container</typeparam>
    public static void AddError<TContainer, TError>(this ref TContainer container, TError error)
    where TContainer : struct, IUnionContainer
    where TError : IError
        => container.AddError((IError)error);

    
    /// <summary>
    /// Adds an error type of <see cref="CustomErrors.Generic"/> to the container with the specified message
    /// </summary>
    /// <param name="container"></param>
    /// <param name="message"></param>
    /// <typeparam name="TContainer"></typeparam>
    public static void AddError<TContainer>(this ref TContainer container, string message)
    where TContainer : struct, IUnionContainer
        => container.AddError(CustomErrors.Generic(message));

    /// <summary>
    /// Adds an error type of <see cref="CustomErrors.ExceptionWrapperError"/> to the container with the specified exception
    /// </summary>
    /// <param name="container"></param>
    /// <param name="ex"></param>
    /// <typeparam name="TContainer"></typeparam>
    public static void AddError<TContainer>(this ref TContainer container, Exception ex)
    where TContainer : struct, IUnionContainer
        => container.AddError(CustomErrors.Exception(ex));

    
    /// <summary>
    /// Adds a list of errors to the container
    /// </summary>
    /// <param name="container"></param>
    /// <param name="errors"></param>
    /// <typeparam name="TContainer"></typeparam>
    public static void AddErrors<TContainer>(this ref TContainer container, params IError[] errors)
    where TContainer : struct, IUnionContainer
        => container.AddErrors(errors);

    /// <summary>
    /// Adds a list of <see cref="CustomErrors.Generic"/> errors to the container with the specified messages
    /// </summary>
    /// <param name="container"></param>
    /// <param name="messages"></param>
    /// <typeparam name="TContainer"></typeparam>
    public static void AddErrors<TContainer>(this ref TContainer container, params string[] messages)
    where TContainer : struct, IUnionContainer
    {
        foreach (string message in messages)
        {
            container.AddError(message);
        }
    }

    /// <summary>
    /// Adds a list of <see cref="TError"/> errors to the container
    /// </summary>
    /// <param name="container"></param>
    /// <param name="errors"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TError"></typeparam>
    public static void AddErrors<TContainer, TError>(this ref TContainer container, params TError[] errors)
    where TContainer : struct, IUnionContainer
    where TError : struct, IError
    {
        foreach (TError error in errors)
        {
            container.AddError(error);
        }
    }

    /// <summary>
    /// Adds a list of <see cref="CustomErrors.ExceptionWrapperError"/> errors to the container with the specified exceptions
    /// </summary>
    /// <param name="container"></param>
    /// <param name="exceptions"></param>
    /// <typeparam name="TContainer"></typeparam>
    public static void AddErrors<TContainer>(this ref TContainer container, params Exception[] exceptions)
    where TContainer : struct, IUnionContainer
    {
        foreach (Exception e in exceptions)
        {
            container.AddError(e);
        }
    }
#endregion

}