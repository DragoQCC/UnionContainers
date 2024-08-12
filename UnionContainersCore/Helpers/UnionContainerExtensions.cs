using UnionContainers.Core.Common;
using UnionContainers.Core.UnionContainers;
using Microsoft.Extensions.Logging;

namespace UnionContainers.Core.Helpers;

public static partial class UnionContainerExtensions
{
    public static UnionContainer<T> ToContainer<T>(this T? item)
    {
        var container = new UnionContainer<T>();
        container.SetValue(item);
        return container;
    }
    
    //Conversion Extensions
    
    /// <summary>
    /// Takes in a container and returns a new container with the same state but includes more generic options
    /// </summary>
    /// <param name="_container"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TTargetContainer"></typeparam>
    /// <returns></returns>
    public static TTargetContainer TryConvertContainer<TContainer,TTargetContainer>(this TContainer _container) 
    where TContainer : UnionContainerBase<TContainer>, IUnionContainer<TContainer> 
    where TTargetContainer : UnionContainerBase<TTargetContainer>, IUnionContainer<TTargetContainer>
    {
        //create a new instance of the target container type
        var newContainer = TTargetContainer.Create();
        try
        {
            //set the value state of the new container to the value state of the source container
            newContainer.ValueState = _container.ValueState;
            newContainer.ErrorState = _container.ErrorState;
            newContainer.ExceptionState = _container.ExceptionState;
            newContainer.EmptyState = _container.EmptyState;
        }
        catch (Exception e)
        {
            if (UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerConversionLogging.Log)
            {
                var logLevel = UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerConversionLogging.LogLevel;
                UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.Logger.Log(logLevel, "Exception while trying to convert container {ContainerType} to target type {TargetContainer} : {Exception}", typeof(TContainer),typeof(TTargetContainer), e);
            }
        }
        return newContainer;
    }
    
    
    /// <summary>
    /// Takes in the current container and a target container type <br/>
    /// Returns a container with the same state as the source container but with the target container generic types
    /// </summary>
    /// <param name="_container"></param>
    /// <param name="newContainer"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TTargetContainer"></typeparam>
    /// <returns></returns>
    public static TTargetContainer TryConvertContainer<TContainer,TTargetContainer>(this TContainer _container, TTargetContainer newContainer) 
    where TContainer : UnionContainerBase<TContainer>, IUnionContainer<TContainer>  
    where TTargetContainer : UnionContainerBase<TTargetContainer>, IUnionContainer<TTargetContainer>
    {
        try
        {
            //set the value state of the new container to the value state of the source container
            newContainer.ValueState = _container.ValueState;
            newContainer.ErrorState = _container.ErrorState;
            newContainer.ExceptionState = _container.ExceptionState;
            newContainer.EmptyState = _container.EmptyState;
        }
        catch (Exception e)
        {
            if (UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerConversionLogging.Log)
            {
                var logLevel = UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerConversionLogging.LogLevel;
                UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.Logger.Log(logLevel, "Exception while trying to convert container {ContainerType} to target type {TargetContainer} : {Exception}", typeof(TContainer),typeof(TTargetContainer), e);
            }
        }
        return newContainer;
    }
    
    
    
    //State Modification Extensions
    
    /// <summary>
    /// Set the Container to a state of containing an exception, if a value is already set it will be overwritten
    /// </summary>
    /// <param name="container"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static TContainer SetException<TContainer>(this TContainer container, Exception exception) where TContainer : UnionContainerBase<TContainer>, IUnionContainer<TContainer>
        => (container.ExceptionState = new(true, exception)).ContinueWith(container as TContainer);
    
    
    
    /// <summary>
    /// Set the Container to a state of containing a empty value, this is the default state of a container <br/>
    /// but can be used to remove an existing result value / return a container to default <br/>
    /// Removes any set value, error or exception
    /// </summary>
    /// <param name="container">The UnionContainer to update</param>
    /// <param name="newEmptyState">Can be set to a NullEmptyType to represent a null response, and can be set to a Empty to represent a void like return where "nothing" is the proper result</param>
    /// <returns></returns>
    public static TContainer SetEmpty<TContainer>(this TContainer container, Empty newEmptyState) where TContainer : UnionContainerBase<TContainer>, IUnionContainer<TContainer>
    {
        container.EmptyState = new(true, newEmptyState);
        container.ValueState = new(false, default);
        container.ErrorState = new(false, default);
        container.ExceptionState = new(false, default);
        return (TContainer)container;
    }
    
    
   
    //State Checking Extensions
    
    /// <summary>
    /// returns true if the container has any errors or exceptions set
    /// </summary>
    /// <param name="container"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static bool HasIssues<TContainer>(this TContainer container) where TContainer : UnionContainerBase<TContainer>,IUnionContainer<TContainer>
        => container.HasErrors() || container.HasException();
    
    
    /// <summary>
    /// Returns `true` if the container is empty, `false` if it has a value
    /// Also returns false when <see cref="UnionContainerConfiguration.ContainersNotEmptyIfIssues"/> is set to true and the container has issues (i.e. an error or exception)
    /// </summary>
    /// <param name="container"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static bool IsEmpty<TContainer>(this TContainer container) where TContainer : UnionContainerBase<TContainer>,IUnionContainer<TContainer>
        => UnionContainerConfiguration.UnionContainerOptionsInternal.ContainersNotEmptyIfIssues 
            ? container.HasIssues() is false && container.HasResult() is false  
            : container.EmptyState.State;
    
    /// <summary>
    /// Returns `true` if the container has an error, `false` if it does not
    /// A container has an error once the <see cref="UnionContainerBase{TContainer}.AddError{TError}"/> method has been called
    /// </summary>
    /// <param name="container"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static bool HasErrors<TContainer>(this TContainer container) where TContainer : UnionContainerBase<TContainer>,IUnionContainer<TContainer>
        => container.ErrorState.State;
   
    /// <summary>
    /// returns true if the container has an exception set, false otherwise
    /// </summary>
    /// <param name="container"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static bool HasException<TContainer>(this TContainer container) where TContainer : UnionContainerBase<TContainer>,IUnionContainer<TContainer>
        => container.ExceptionState.State;
    
    /// <summary>
    /// Returns true if the container has a value set, false otherwise
    /// </summary>
    /// <param name="container"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static bool HasResult<TContainer>(this TContainer container) where TContainer : UnionContainerBase<TContainer>,IUnionContainer<TContainer>
        => container.ValueState.State;
    
    
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
    public static TContainer IfEmptyDo<TContainer>(this TContainer container, Action action) where TContainer : UnionContainerBase<TContainer>,IUnionContainer<TContainer>
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
    public static TContainer IfExceptionDo<TContainer>(this TContainer container, Action<Exception> action) where TContainer : UnionContainerBase<TContainer>,IUnionContainer<TContainer>  
    {
        try
        {
            if(container.HasException())
            {
                return container.GetException().IfNotNullDo(action).ContinueWith(container as TContainer);
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

    
    //Data Access Extensions
    /// <summary>
    /// Returns the exception of the container if one is set, otherwise returns null
    /// </summary>
    /// <param name="container"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <returns></returns>
    public static Exception? GetException<TContainer>(this TContainer container) where TContainer : UnionContainerBase<TContainer>,IUnionContainer<TContainer>
        => container.HasException() ? container.ExceptionState.Exception! : null;
    
    
    
    public static UnionContainer<TResult> TryHandleResult<TResult>(this UnionContainer<TResult> container, Action<TResult> action, Action<Exception>? catchHandler = null) 
    {
        UnionContainer<TResult,bool> resultValidationContainer = container.TryObtainResult<TResult>();
        _ = resultValidationContainer.ValueState.Value switch
        {
            bool shortCircuit => Empty.Nothing,
            TResult value => new Func<Empty>(() =>
            {
                try
                {
                    action!.TryCatch(value, catchHandler);
                    container.MatchedThisCycle = true;
                }
                catch (Exception e)
                {
                   container.LogTryHandleException(e);
                }
                return Empty.Nothing;
            })(),
            _ => Empty.Nothing
        };
        return container;
    }
    
    internal static void LogMethodToContainerException<TContainer>(this TContainer container,Exception e) where TContainer : UnionContainerBase<TContainer>,IUnionContainer<TContainer>
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
    }
    
}