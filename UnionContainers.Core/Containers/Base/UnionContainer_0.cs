using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnionContainers.Errors;

namespace UnionContainers.Containers.Base;

public interface IUnionContainer
{
    public UnionContainerState State { get; init; }
    internal List<IError>? Errors { get; set; }
    
    
    public void AddError(IError error)
    {
        Errors ??= [];
        Errors.Add(error);
    }
    
    public void AddErrors(params IError[] errors)
    {
        Errors ??= [];
        Errors.AddRange(errors);
    }
    
    public bool HasErrors() => Errors?.Count > 0;
    
    public List<IError> GetErrors() => Errors ?? new();

    public List<TError> GetErrors<TError>()
    {
        Errors ??= [];
        if (Errors.Count == 0)
        {
            return [];
        }
        var returnList = new List<TError>();
        foreach (var error in Errors)
        {
            if (error is TError errorValue)
            {
                returnList.Add(errorValue);
            }
        }
        return returnList;
    }
    
    public void ForState(UnionContainerState state, Action action)
    {
        if (this.State == state)
        {
            action();
        }
    }
    
    public void ForState(Action? defaultAction = null, params ValueTuple<UnionContainerState,Action>[] targetState)
    {
        foreach (var (state, action) in targetState)
        {
            if (this.State == state)
            {
                action();
            }
            else
            {
                defaultAction?.Invoke();
            }
        }
    }
    
}


public interface IUnionContainer<TContainer> : IUnionContainer where TContainer : IUnionContainer<TContainer>;

public interface IUnionResultContainer<TValueTuple> : IUnionContainer<IUnionResultContainer<TValueTuple>>
    where TValueTuple : struct, ITuple
{
    internal TValueTuple ResultValue { get; init; }
}



/*/// <summary>
/// A result type wrapper that can contain one of the supplied value types <br/>
/// Can also contain any exceptions produced by functions ran by the container or any custom error messages set by the user <br/>
/// </summary>
/// <typeparam name="TContainer"></typeparam>
[Serializable]
[DataContract]
public abstract record UnionContainerBase<TContainer,TValueTuple,TError> : ISerializable
    where TContainer : UnionContainerBase<TContainer,TValueTuple,TError>, IUnionContainer<TContainer>
    where TValueTuple : struct, ITuple
{

    public UnionContainerState State { get; internal set; } = UnionContainerState.Empty;
    internal TValueTuple Result { get; init; }
    internal List<TError>? Errors { get; set; } = null;
    internal Exception? Exception { get; init; } = null;



    /*[DataMember]
    [JsonInclude]
    internal ValueState ValueState { get;  set; }

    [DataMember]
    [JsonInclude]
    internal ErrorState ErrorState { get;  set; }

    [DataMember]
    [JsonInclude]
    internal ExceptionState ExceptionState { get;  set; }

    [DataMember]
    [JsonInclude]
    internal EmptyState EmptyState { get;  set; }#1#

    //prevents external instantiation & inheritance
    internal UnionContainerBase()
    {
    }

    internal UnionContainerBase(object? value)
    {
        //Might remove this one in favor of just create & factory methods to avoid boxing

    }


    /// <summary>
    /// Returns the container result value  <br/>
    /// takes an optional backup value to return when the container does not have a result or the result is null <br/>
    /// Otherwise, the default of the result type is returned. <br/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public TResult? TryGetValue<TResult>(TResult? fallbackValue = default, Func<TResult>? fallbackValueMethod = null)
    {
        if (((TContainer)this).HasResult())
        {
            if(ValueState.Value is TResult result)
            {
                return result;
            }
        }
        if(fallbackValue is not null)
        {
            return fallbackValue;
        }
        if(fallbackValueMethod is not null)
        {
            return fallbackValueMethod();
        }
        return default;
    }


    /// <summary>
    /// Should be rarely used, only use if the type of the container is unknown <br/>
    /// Invokes the <see cref="TryGetValue{TResult}"/> method and returns the result as an object <br/>
    /// </summary>
    /// <returns></returns>
    public object? TryGetValue() =>  TryGetValue<object>();


    /// <summary>
    /// Takes an Action delegate to try and handle the result type of the container.
    /// <code>
    /// //Create a container
    /// UnionContainer{string,int} container = 5; <br/>
    /// //this will print "The value is an int 5" <br/>
    /// container.MatchResult((int value) => Console.WriteLine($"The value is an int {value}")); <br/>
    /// </code>
    /// </summary>
    /// <param name="action">The method to execute if the container result is of type T</param>
    /// <param name="catchHandler">An optional method to handle an exception produced during the execution of the first function</param>
    /// <typeparam name="T">The type to match the UnionContainer result too, if the container result is a different type the method will not execute</typeparam>
    /// <returns>The current UnionContainer, allows for easy method chaining</returns>
    public TContainer MatchResult<T>(Action<T> action, Action<Exception>? catchHandler = null)
    {
        UnionContainer<T> resultValidationContainer = GetResultIfValid<T>();

        _ = resultValidationContainer.State switch
        {
            UnionContainerState.Result => new Func<Empty>(() =>
            {
                try
                {
                    T value = resultValidationContainer.Result.Item1;
                    action!.TryCatch(value, catchHandler);
                }
                catch (Exception e)
                {
                    LogTryHandleException(e);
                }
                return Empty.Nothing;
            })(),
            _ => Empty.Nothing
        };
        return (TContainer)this;
    }


    /// <summary>
    /// Takes in a function to handle the containers result value. <br/>
    /// Can return an arbitrary value based on the supplied function.
    /// <code>
    /// <![CDATA[
    /// //Create a container
    /// UnionContainer<Employee> container = Method();
    /// //this will store the employees name if the container result type is an Employe
    /// container.MatchResult((Employee employee) => employee.Name);
    /// string EmployeeName = container.GetMatchedItemAs<string>();
    /// ]]>
    /// </code>
    /// </summary>
    /// <param name="function">The main Func to execute if the container holds a result of type T</param>
    /// <param name="catchHandler">An optional method to handle an exception produced during the execution of the first function, needs to return a type of T</param>
    /// <param name="fallbackValue">An optional value to return if the first function fails or the container result is not of type T, used to ensure a non-null result when <see cref="GetMatchedItemAs{T}"/> is invoked</param>
    /// <typeparam name="TMatch">The type that should be stored in the container and used for executing the passed in methods</typeparam>
    /// <typeparam name="T">The type to store as the result from the passed in Funcs to be used with <see cref="GetMatchedItemAs{T}"/> later</typeparam>
    /// <returns>The current UnionContainer, allows for easy method chaining</returns>
    public T? MatchResult<TMatch,T>(Func<TMatch,T?> function, Func<Exception,T>? catchHandler = null, T? fallbackValue = default)
    {
        UnionContainer<TMatch> resultValidationContainer = GetResultIfValid<TMatch>();

        return resultValidationContainer.State switch
        {
            UnionContainerState.Result => new Func<T?>(() =>
            {
                try
                {
                    TMatch value = resultValidationContainer.Result.Item1;
                    return function!.TryCatch(value, catchHandler);
                }
                catch (Exception e)
                {
                    LogTryHandleException(e);
                }
                return fallbackValue ?? default;
            })(),
            _ => fallbackValue ?? default
        };
    }


    /// <summary>
    /// Sets the state of the container to an error, and adds the error value to the container. <br/>
    /// Errors are set as the supplied generic type, users should ensure the same type is used for all error values for the life of the container. <br/>
    /// </summary>
    /// <param name="error"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    /*public TContainer AddError<TError>(TError error) where TError : notnull
    {
        try
        {
            if (ErrorState.State)
            {
                //check if the error type is assignable to the existing error type
                if (ErrorState.ErrorItems.Count > 0)
                {
                    var errorType = ErrorState.ErrorItems.First().GetType();
                    #if NETCOREAPP
                        bool isAssignable = error.GetType().IsAssignableTo(errorType);
                    #else
                        isAssignable = errorType.GetType().IsAssignableFrom(error.GetType());
                    #endif
                    if (isAssignable is false)
                    {
                        throw new ArgumentException($"Error value type {error.GetType()} is not assignable to existing error type {errorType}");
                    }
                }
                ErrorState.ErrorItems.Add(error);
            }
            else
            {
                ErrorState = new(true, [error]);
            }
        }
        catch (Exception e)
        {
            if (UnionContainerOptionsInternal.LoggingOptions.ContainerModificationLogging.Log)
            {
                var logLevel = UnionContainerOptionsInternal.LoggingOptions.ContainerModificationLogging.LogLevel;
                UnionContainerOptionsInternal.LoggingOptions.Logger.Log(logLevel, "Exception while trying to add error value {Exception}", e);
            }
        }
        return (TContainer)this;
    }#1#

    /// <summary>
    /// Tries to Add the provided errors to the container using <see cref="AddError{TError}"/>
    /// </summary>
    /// <param name="errorValues"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public TContainer AddErrors<TError>(params TError[] errorValues) where TError : notnull
    {
        foreach (var errorValue in errorValues)
        {
            AddError(errorValue);
        }
        return (TContainer)this;
    }


    /// <summary>
    /// Returns the containers stored error values as the specified generic type <br/>
    /// Should be the same generic type used when setting the error values <br/>
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    /*public List<TError> GetErrors<TError>()
    {
        List<TError> errorList = [];
        bool shortCircuit = ((TContainer)this).HasErrors() is false;
        if (shortCircuit is false)
        {
            try
            {
                errorList  = ErrorState.ErrorItems.Cast<TError>().ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while trying to get error values as specified type {typeof(TError)}");
                Console.WriteLine(e);
            }
        }
        return errorList;
    }#1#

    public void AddError(TError error)
    {
        if (Errors is not null)
        {
            Errors.Add(error);
        }
        else
        {
            Errors = new() {error};
        }
    }


    public List<TError> GetErrors() => Errors ?? new();

    /// <summary>
    /// Takes in an action to handle the containers error values and one for the container exception. <br/>
    /// Supplied actions are only invoked if the container has an error or exception state respectively. <br/>
    /// </summary>
    /// <param name="isError"></param>
    /// <param name="isException"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public TContainer HandleIssues<TError>(Action<List<TError>> isError, Action<Exception> isException)
        => this switch
        {
            {ErrorState.State: true} => IfErrorDo(isError),
            {ExceptionState.State: true} => ((TContainer)this).IfExceptionDo(isException),
            _ => (TContainer)this
        };


    /// <summary>
    /// Takes a supplied action to handle the containers errors if the container has an error state. <br/>
    /// </summary>
    /// <param name="action"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public TContainer IfErrorDo<TError>(Action<List<TError>> action)
    {
        try
        {
            if(((TContainer)this).HasErrors())
            {
                action(GetErrors<TError>());
            }
        }
        catch (Exception e)
        {
            if (UnionContainerOptionsInternal.LoggingOptions.ContainerErrorHandlingLogging.Log)
            {
                var logLevel = UnionContainerOptionsInternal.LoggingOptions.ContainerErrorHandlingLogging.LogLevel;
                UnionContainerOptionsInternal.LoggingOptions.Logger.Log(logLevel, "Exception while trying to handle container error value: {Exception}", e);
            }
            if (UnionContainerOptionsInternal.TreatExceptionsAsErrors)
            {
                AddError("Error while trying to handle error state, " + e);
            }
            if(UnionContainerOptionsInternal.ThrowExceptionsFromUserHandlingCode)
            {
                throw;
            }
        }
        return (TContainer)this;
    }


    /#1#// <summary>
    /// Sets the value state of the container to the supplied value
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public TContainer SetValue<TValue>(TValue? value)
    {
        if (value is null || value is Empty)
        {
            return (TContainer)this;
        }
        if(UnionContainerOptionsInternal.DefaultAsNull && value.Equals(default(TValue)))
        {
            return (TContainer)this;
        }
        ValueState = new(true, value);
        EmptyState = new(false, Empty.Nothing);
        return (TContainer)this;
    }#1#


    /// <summary>
    /// Should only be invoked on concrete container types, not the base type <br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static UnionContainer<object> Create() => throw new NotImplementedException();


    /// <summary>
    /// Takes in a container and returns a new container with the same state but includes more generic options
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TTargetContainer"></typeparam>
    /// <returns></returns>
    public TTargetContainer TryConvertContainer<TTargetContainer>() where TTargetContainer : UnionContainerBase<TTargetContainer>, IUnionContainer<TTargetContainer>
    {
        //create a new instance of the target container type
        var newContainer = TTargetContainer.Create();
        try
        {
            //set the states of the new container to the states of the source container
            newContainer.ValueState = ValueState;
            newContainer.ErrorState = ErrorState;
            newContainer.ExceptionState = ExceptionState;
            newContainer.EmptyState = EmptyState;
        }
        catch (Exception e)
        {
            if (UnionContainerOptionsInternal.LoggingOptions.ContainerConversionLogging.Log)
            {
                var logLevel = UnionContainerOptionsInternal.LoggingOptions.ContainerConversionLogging.LogLevel;
                UnionContainerOptionsInternal.LoggingOptions.Logger.Log(logLevel, "Exception while trying to convert container {ContainerType} to target type {TargetContainer} : {Exception}", typeof(TContainer),typeof(TTargetContainer), e);
            }
        }
        return newContainer;
    }


     ///<summary>
     /// ! Used for serialization only, should not be invoked directly
     /// </summary>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("ValueState", ValueState);
        info.AddValue("ErrorState", ErrorState);
        info.AddValue("ExceptionState", ExceptionState);
        info.AddValue("EmptyState", EmptyState);
    }



    /// <summary>
    /// Attempts to return the container result type as the specified generic type <br/>
    /// If a boolean is returned the container either did not have a non-null result, or the result could not be converted to the specified type <br/>
    /// When <see cref="UnionContainerOptions.DefaultAsNull"/> is true this method will also return a boolean if the result is the default value of the specified type <br/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    internal UnionContainer<TResult> GetResultIfValid<TResult>()
    {
        if (((TContainer)this).HasResult())
        {
            try
            {
                TResult? value = TryGetValue<TResult>();
                if (value.IsNull() || UnionContainerOptionsInternal.DefaultAsNull && value.IsDefault())
                {
                    return Empty.Nothing;
                }
                return value;
            }
            catch(Exception e)
            {
                return e;
            }
        }
        return Empty.Nothing;
    }

    internal Empty LogTryHandleException(Exception e)
    {
        if (UnionContainerOptionsInternal.LoggingOptions.ContainerResultHandlingLogging.Log)
        {
            var logLevel = UnionContainerOptionsInternal.LoggingOptions.ContainerResultHandlingLogging.LogLevel;
            UnionContainerOptionsInternal.LoggingOptions.Logger.Log(logLevel, "Exception while trying to handle result {Exception}", e);
        }
        if (UnionContainerOptionsInternal.TreatExceptionsAsErrors)
        {
            AddError("Error while trying to handle result, " + e);
        }
        if(UnionContainerOptionsInternal.ThrowExceptionsFromUserHandlingCode)
        {
            throw e;
        }
        return Empty.Nothing;
    }

}*/


