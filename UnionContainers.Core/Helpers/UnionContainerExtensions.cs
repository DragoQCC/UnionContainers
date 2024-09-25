using System.Runtime.CompilerServices;
using HelpfulTypesAndExtensions;
using Microsoft.Extensions.Logging;

namespace UnionContainers;

public static class UnionContainerExtensions
{
    #region Conversion Extensions
    //Conversion Extensions
    public static UnionContainer<T> ToContainer<T>(this T? item) => item is null ? UnionContainer<T>.Empty() : new(item);
    
    
    /*/// <summary>
    /// Takes in the current container and a target container type <br/>
    /// Returns a container with the same state as the source container but with the target container generic types <br/>
    /// The idea is when you have a container of a lesser generic count and need to convert it to a container with more generic types <br/>
    /// Ex. Pass a UnionContainer{int} to a method wanting a UnionContainer{int,string} <br/>
    /// </summary>
    /// <param name="container"></param>
    /// <param name="newContainer"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TTargetContainer"></typeparam>
    /// <returns></returns>
    public static TTargetContainer TryConvertContainer<TContainer,TTargetContainer>(this TContainer container) 
        where TContainer : struct, IUnionContainer
        where TTargetContainer : struct, IUnionContainer
    {
        try
        {
            TTargetContainer newContainer = new TTargetContainer();
            newContainer.State = container.State;
            newContainer.Errors = container.Errors;
            newContainer.ResultValue = container.ResultValue;
        }
        catch (Exception e)
        {
            if (UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerConversionLogging.Log)
            {
                var logLevel = UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerConversionLogging.LogLevel;
                UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.Logger.Log(logLevel, "Exception while trying to convert container {ContainerType} to target type {TargetContainer} : {Exception}", typeof(TContainer),typeof(TTargetContainer), e);
            }
            throw;
        }
        return newContainer;
    }*/
    
    #endregion
   
    #region State Checking Extensions
    //State Checking Extensions
    
    /// <summary>
    /// returns true if the container has any errors or exceptions set
    /// </summary>
    /// <param name="container"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static bool MissingResult<TContainer>(this TContainer container) where TContainer : struct, IUnionContainer
        => container.HasResult() is false;
    
    
    /// <summary>
    /// Returns `true` if the container is empty, `false` if it has a value
    /// Also returns false when <see cref="UnionContainerConfiguration.ContainersNotEmptyIfIssues"/> is set to true and the container has issues (i.e. an error or exception)
    /// </summary>
    /// <param name="container"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static bool IsEmpty<TContainer>(this TContainer container) where TContainer : struct, IUnionContainer
        => UnionContainerConfiguration.UnionContainerOptionsInternal.ContainersNotEmptyIfIssues 
            ? container.HasErrors() 
            : container.State is UnionContainerState.Empty;
    
    /// <summary>
    /// Returns `true` if the container has an error, `false` if it does not
    /// A container has an error once the <see cref="UnionContainerBase{TContainer}.AddError{TError}"/> method has been called
    /// </summary>
    /// <param name="container"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static bool HasErrors<TContainer>(this TContainer container) where TContainer : struct, IUnionContainer
        => container.State is UnionContainerState.Error;
    
    
    /// <summary>
    /// Returns true if the container has a value set, false otherwise
    /// </summary>
    /// <param name="container"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static bool HasResult<TContainer>(this TContainer container) where TContainer : struct, IUnionContainer
        => container.State is UnionContainerState.Result;
    
    public static UnionContainerState GetState<TContainer>(this TContainer container) where TContainer : IUnionContainer //struct, IUnionContainer
        => container.State;
    
    #endregion
    
    #region State Handle Extensions
    //State Handling Extensions
    
    /// <summary>
    /// Executes the supplied action if the container is still empty <br/>
    /// Container is empty if it has no value, error or exception set <br/>
    /// Throws an exception if the action throws an exception and <see cref="UnionContainerOptions.ThrowExceptionsFromUserHandlingCode"/> is set to true <br/>
    /// </summary>
    /// <param name="container"></param>
    /// <param name="action"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns>The original container instance so method calls can be chained</returns>
    public static TContainer IfEmptyDo<TContainer>(this ref TContainer container, Action action) where TContainer : struct, IUnionContainer
    {
        try
        {
            if(container.IsEmpty())
            {
                container.IfNotNullDo(action).ContinueWith(container);
            }
        }
        catch (Exception e)
        {
            if (UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerErrorHandlingLogging.Log)
            {
                var logLevel = UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerErrorHandlingLogging.LogLevel;
                UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.Logger.Log(logLevel, "Exception while trying to handle empty state {Exception}", e);
            }
            if (UnionContainerConfiguration.UnionContainerOptionsInternal.TreatExceptionsAsErrors)
            {
                container.AddError("Error while trying to handle empty state, " + e);
            }
            if(UnionContainerConfiguration.UnionContainerOptionsInternal.ThrowExceptionsFromUserHandlingCode)
            {
                throw;
            }
        }
        return container;
    }
    
    
    /// <summary>
    /// Executes the supplied action if the container has an exception set
    /// Throws an exception if the action throws an exception and <see cref="UnionContainerOptions.ThrowExceptionsFromUserHandlingCode"/> is set to true <br/>
    /// </summary>
    /// <param name="container"></param>
    /// <param name="action"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static TContainer IfExceptionDo<TContainer>(this ref TContainer container, Action<Exception> action) where TContainer : struct, IUnionContainer
    {
        try
        {
            Exception? ex = container.GetException();
            if(ex is not null)
            {
                action(ex);
            }
        }
        catch (Exception e)
        {
            if (UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerExceptionHandlingLogging.Log)
            {
                var logLevel = UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerExceptionHandlingLogging.LogLevel;
                UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.Logger.Log(logLevel, "Exception produced while handling containers exception value {Exception}", e);
            }
            if (UnionContainerConfiguration.UnionContainerOptionsInternal.TreatExceptionsAsErrors)
            {
                container.AddError("Error while trying to handle exception state, " + e);
            }
            if(UnionContainerConfiguration.UnionContainerOptionsInternal.ThrowExceptionsFromUserHandlingCode)
            {
                throw;
            }
        }
        return container;
    }
    
    #endregion

    
    #region Data Access Extensions
    //Data Access Extensions
    /// <summary>
    /// Returns the exception of the container if one is set, otherwise returns null
    /// </summary>
    /// <param name="container"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static Exception? GetException<TContainer>(this TContainer container) where TContainer : IUnionContainer
        => container.State switch
        {
            UnionContainerState.Error => container.GetErrors<CustomErrors.ExceptionWrapperError>().FirstOrDefault().exception.ReturnIf(x => x.IsNotDefault()),
            _ => null
        };

    
    
    
    public static void AddError<TContainer>(this ref TContainer container, IError error) where TContainer : struct, IUnionContainer
    {
        container.Errors ??= [];
        container.Errors.Add(error);
        if(container.State != UnionContainerState.Error)
        {
            container.State = UnionContainerState.Error;
        }
    }
    
    public static void AddError<TContainer,TError>(this ref TContainer container, TError error) where TContainer : struct, IUnionContainer where TError : IError
     => container.AddError((IError)error);
    
    public static void AddError<TContainer>(this ref TContainer container, string message) where TContainer : struct, IUnionContainer
        => container.AddError(CustomErrors.Generic(message));
    
    public static void AddError<TContainer>(this ref TContainer container, Exception ex) where TContainer : struct, IUnionContainer
        => container.AddError(CustomErrors.Exception(ex));
    
   
    
    public static void AddErrors<TContainer>(this ref TContainer container, params IError[] errors) where TContainer : struct, IUnionContainer
        => container.AddErrors(errors);

    public static void AddErrors<TContainer>(this ref TContainer container, params string[] messages)
        where TContainer : struct, IUnionContainer
    {
        foreach (var message in messages)
        {
            container.AddError(message);
        }
    }

    public static void AddErrors<TContainer, TError>(this ref TContainer container, params TError[] errors)
        where TContainer : struct, IUnionContainer where TError : struct, IError
    {
        foreach (var error in errors)
        {
           container.AddError(error); 
        }
    }

    public static void AddErrors<TContainer>(this ref TContainer container, params Exception[] exceptions)
        where TContainer : struct, IUnionContainer
    {
        foreach (var e in exceptions)
        {
            container.AddError(e);
        }
    }
    
    
    
    public static List<IError> GetErrors<TContainer>(this  TContainer container) where TContainer : IUnionContainer//struct, IUnionContainer
        => container.GetErrors();
    
    public static List<TError> GetErrors<TError>(this IUnionContainer container)  where TError : struct, IError
        => container.GetErrors<TError>();
    
    
    
    internal static TContainer TryCreateResult<TContainer,TValueTuple>(this ref TContainer container, TValueTuple value) where TContainer : struct, IUnionResultContainer<TValueTuple> where TValueTuple : struct, ITuple
    {
        if (container.State != UnionContainerState.Empty)
        {
            return container;
        }
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] is not null)
            {
                container = container with { ResultValue = value, State = UnionContainerState.Result };
                break;
            }
        }
        return container;
    }
    
    public static void Match<TContainer,TValueTuple>(this TContainer container, Action<Exception>? catchHandler = null, params Delegate[] actions) where TContainer: IUnionResultContainer<TValueTuple> where TValueTuple : struct,ITuple
    {
        if (container.State != UnionContainerState.Result)
        {
            return;
        }
        for (int i = 0; i < container.ResultValue.Length; i++)
        {
            if (container.ResultValue[i] is null)
            {
                continue;
            }
            try
            {
                var result = container.ResultValue[i];
                if (result.GetType() == actions[i].Method.GetParameters()[0].ParameterType)
                {
                    actions[i].DynamicInvoke(result);
                }
                break;
            }
            catch (Exception e)
            {
                catchHandler?.Invoke(e);
            }
        }
    }
    
    /*internal static void LogMethodToContainerException<TContainer>(this TContainer container,Exception e) where TContainer : UnionContainerBase<TContainer>,IUnionContainer<TContainer>
    {
        if (UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerCreationLogging.Log)
        {
            var logLevel = UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerCreationLogging.LogLevel;
            UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.Logger.Log(logLevel, "Exception occured during method execution while creating container: {Exception}", e);
        }
        if (UnionContainerConfiguration.UnionContainerOptionsInternal.TreatExceptionsAsErrors)
        {
            container.AddError("Error while trying to execute method, " + e);
        }
        else
        {
            container.SetException(e);
        }
    }*/
    
    #endregion
}