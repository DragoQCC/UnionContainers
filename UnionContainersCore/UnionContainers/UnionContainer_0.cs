using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using UnionContainers.Core.Common;
using UnionContainers.Core.Helpers;
using UnionContainers.Shared.Common;
using static UnionContainers.Core.Common.UnionContainerConfiguration;

namespace UnionContainers.Core.UnionContainers;

public interface IUnionContainer<TContainer> where TContainer : IUnionContainer<TContainer>
{
    public abstract static TContainer Create();
}


/// <summary>
/// Is a UnionContainer{object} type <br/>
/// Should be rarely used, only use if the type of the container is unknown <br/>
/// Otherwise a <see cref="UnionContainerBase{TContainer}"/> or child type should be used <br/>
/// </summary>
public interface IUnionContainer : IUnionContainer<UnionContainer<object>>
{
    /// <summary>
    /// Returns the containers result value, if no value is set then a <see cref="InvalidOperationException"/> should be thrown. <br/> 
    /// </summary>
    /// <returns></returns>
    public object? TryGetValue();
}


/// <summary>
/// A result type wrapper that can contain one of the supplied value types <br/>
/// Can also contain any exceptions produced by functions ran by the container or any custom error messages set by the user <br/>
/// </summary>
/// <typeparam name="TContainer"></typeparam>
[Serializable]
[DataContract]
public abstract record UnionContainerBase<TContainer> : IUnionContainer, ISerializable where TContainer : UnionContainerBase<TContainer> , IUnionContainer<TContainer>
{
    [DataMember]
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
    internal EmptyState EmptyState { get;  set; }
    
    internal bool MatchedThisCycle {get; set;}
    private object? ItemMatchedThisCycle {get; set;}
    
    //prevents external instantiation & inheritance
    internal UnionContainerBase()
    {
        ValueState = new(false, null);
        EmptyState = new(true, Empty.Nothing);
        ErrorState = new(false, []);
        ExceptionState = new(false, null);
    }
    
    internal UnionContainerBase(object? value)
    {
        if (value is not null)
        {
            ValueState = new(true, value);
            EmptyState = new(false, Empty.Nothing);
        }
        else
        {
            ValueState = new(false, null);
            EmptyState = new(true, Empty.Nothing);
        }
        ErrorState = new(false, null);
        ExceptionState = new(false, null);
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
    /// container.TryHandleResult((int value) => Console.WriteLine($"The value is an int {value}")); <br/>
    /// </code>
    /// </summary>
    /// <param name="action">The method to execute if the container result is of type T</param>
    /// <param name="catchHandler">An optional method to handle an exception produced during the execution of the first function</param>
    /// <typeparam name="T">The type to match the UnionContainer result too, if the container result is a different type the method will not execute</typeparam>
    /// <returns>The current UnionContainer, allows for easy method chaining</returns>
    public TContainer TryHandleResult<T>(Action<T> action, Action<Exception>? catchHandler = null) 
    {
        UnionContainer<T,bool> resultValidationContainer = TryObtainResult<T>();
        _ = resultValidationContainer.ValueState.Value switch
        {
            bool shortCircuit => Empty.Nothing,
            T value => new Func<Empty>(() =>
            {
                try
                {
                    action!.TryCatch(value, catchHandler);
                    MatchedThisCycle = true;
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
    /// container.TryHandleResult((Employee employee) => employee.Name);
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
    public TContainer TryHandleResult<TMatch,T>(Func<TMatch,T?> function, Func<Exception,T>? catchHandler = null, T? fallbackValue = default) 
    {
        UnionContainer<TMatch,bool> resultValidationContainer = TryObtainResult<TMatch>();
        
        _ = resultValidationContainer.ValueState.Value switch
        {
            bool shortCircuit => new Func<Empty>(() =>
            {
                if(fallbackValue.IsNullOrDefault() is false)
                {
                    StoreMatchedItem(fallbackValue);
                }
                return Empty.Nothing;
            })(),
            TMatch value => new Func<Empty>(() =>
            {
                try
                {
                    T? methodResult = function!.TryCatch(value, catchHandler);
                    StoreMatchedItem(methodResult);
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
    /// Takes a list of delegates to try and handle the result type of the container. <br/>
    /// Will automatically convert the container value to the result type if possible and execute the action if the result type matches the container value type. <br/>
    /// Example:
    /// <code>
    /// //Create a container
    /// UnionContainer{string,int} container = new().SetValue(5);
    /// //this will print "The value is an int 5"
    /// container.TryHandleResult(
    ///    (int value) => Console.WriteLine($"The value is an int {value}"),
    ///    (string value) => Console.WriteLine($"The value is a string {value}"), 
    ///    //this type will never be executed because the container cannot be this type 
    ///    (double value) => Console.WriteLine($"The value is a double {value}")
    /// );
    /// </code>
    /// </summary>
    /// <param name="actions"></param>
    /// <returns></returns>
    public TContainer TryHandleResult(params Delegate[] actions)
    {
        try
        {
            bool shortCircuit = ((TContainer)this).HasResult() is false;
            //a container has a inheritance of IResultTypeWrapper<Tn,IResultn> for each value type it can contain
            var resultTypeWrappers = GetType().GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IResultTypeWrapper<,>));
            //for each wrapper interface check if the generic type parameter matches the type of the value state
            object? containerResult = default;
            try
            {
                containerResult = TryGetValue();
            }
            catch
            {
                shortCircuit = true;
            }
            if (containerResult is null)
            {
                shortCircuit = true;
            }
            if(shortCircuit is false)
            {
                foreach (var resultTypeWrapper in resultTypeWrappers)
                {
                    //get the generic type parameter of the IResultTypeWrapper<Tn,IResultn> interface
                    var resultType = resultTypeWrapper.GetGenericArguments().First();
                    try
                    {
                        //try to convert the container value to the resultType, if the containerResult is a json element then deserialize it
                        if (containerResult is JsonElement jsonElement)
                        {
                            containerResult = JsonSerializer.Deserialize(jsonElement.GetRawText(), resultType);
                        }
                        else
                        {
                            containerResult = Convert.ChangeType(containerResult, resultType);
                        }
                    }
                    catch
                    {
                        //this only gets hit if the conversion fails which typically means its not the correct type
                        continue;
                    }
                    //if the container value matches the resultType then execute the action
                    //get the action that matches the resultType
                    var action = actions.FirstOrDefault(x => x?.GetMethodInfo().GetParameterTypes().Count(y => y == resultType) > 0);
                    //if the action is not null then execute it
                    if (action is not null)
                    {
                        action.DynamicInvoke(containerResult);
                    }
                }
            }
        }
        catch (Exception e)
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
                throw;
            }
        }
        return (TContainer)this;
    }
   
    
    /// <summary>
    /// Sets the state of the container to an error, and adds the error value to the container. <br/>
    /// Errors are set as the supplied generic type, users should ensure the same type is used for all error values for the life of the container. <br/>
    /// </summary>
    /// <param name="error"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public TContainer AddError<TError>(TError error) where TError : notnull
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
    }
    
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
    /// Attempts to add an error value to the container, if the error value is not of the same type as the existing error values then an exception is thrown. <br/>
    /// This exception is caught by the container but if configured to throw exceptions from user code then the exception is rethrown. <br/>
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public TContainer AddError(dynamic error)
    {
        this.AddError<dynamic>(error);
        return (TContainer)this;
    }
    
    
    /// <summary>
    /// Tries to Add the provided errors to the container using <see cref="AddError"/>
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public TContainer AddErrors(params dynamic[] errors)
    {
        foreach (dynamic error in errors)
        {
            this.AddError(error);
        }
        return (TContainer)this;
    }
    
    
    /// <summary>
    /// Returns the containers stored error values as the specified generic type <br/>
    /// Should be the same generic type used when setting the error values <br/>
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public List<TError> GetErrors<TError>()
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
    }
    
    /// <summary>
    /// Returns the containers stored error values as a dynamic type <br/>
    /// Can be used in place of <see cref="GetErrors{TError}"/> if the error values are not known <br/>
    /// Returns a empty list if no error values are set <br/>
    /// </summary>
    /// <returns></returns>
    public List<dynamic> GetErrors()
    {
        return !((TContainer)this).HasErrors() ? [] : ErrorState.ErrorItems;
    }
    
    
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
    
    
    /// <summary>
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
    }
    
    
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
    /// If during calls to <see cref="TryHandleResult{T}"/> the method passed in returns a value then this method can be used to get that value <br/>
    /// Should only be used when a <see cref="Func{TResult}"/> / <see cref="Task{T}"/> that returns a value you want to capture is passed into <see cref="TryHandleResult{T}"/> <br/>
    /// Once a value is returned it is cleared and a new value can be set by invoking <see cref="TryHandleResult{T}"/> with a Func/Task again <br/>
    /// </summary>
    /// <example>
    /// This example creates a <see cref="UnionContainer{T}"/> and uses <see cref="TryHandleResult{T}"/> to handle the result <br/>
    /// During the call to <see cref="TryHandleResult{T}"/> the employee name is returned if the container result type is an Employee <br/>
    /// This returned value is then stored temporarily in the container and can be retrieved using <see cref="GetMatchedItemAs{T}()"/>
    /// <code>
    /// public static void ExampleMethod()
    /// {
    ///    //Create a container
    ///    UnionContainer{string, int} container = new UnionContainer{string, int}().SetValue(5);
    ///    
    ///    int number = container.TryHandleResult(
    ///            (int value) =>
    ///            {
    ///                 Console.WriteLine($"The value is an int");
    ///                 return value;
    ///            })).GetMatchedItemAs{int}();
    ///     Console.WriteLine($"The value is {number}"); // prints 5
    /// }
    /// </code>
    /// </example>
    /// <typeparam name="T">The type to return that was stored from a Func or Tasks output</typeparam>
    /// <returns>Either the result from a Func passed to <see cref="TryHandleResult{T}"/> or the default for the type </returns>
    public T? GetMatchedItemAs<T>()
    {
        if(MatchedThisCycle is false || ItemMatchedThisCycle is not T matchedItem)
        {
            return default;
        }
        ItemMatchedThisCycle = default;
        MatchedThisCycle = false;
        return matchedItem;
    }
    
    
    /// <summary>
    /// Like its generic version <see cref="GetMatchedItemAs{T}"/> but returns a non-generic object that must be cast into the desired type <br/>
    /// If during a call to TryHandleResult a value is returned then this method can be used to get that value <br/>
    /// Once a value is returned it is cleared and a new set of TryHandleResult calls can be used to return new values <br/>
    /// </summary>
    /// <returns>Either the result from a Func passed to <see cref="TryHandleResult{T}"/> or the default for the type</returns>
    /// <inheritdoc cref="GetMatchedItemAs{T}"/>
    public object? GetMatchedItem()
    {
        object? matchedItem = ItemMatchedThisCycle;
        if(MatchedThisCycle is false)
        {
            return default;
        }
        ItemMatchedThisCycle = default;
        MatchedThisCycle = false;
        return matchedItem;
    }
    
    
    /// <summary>
    /// Attempts to return the container result type as the specified generic type <br/>
    /// If a boolean is returned the container either did not have a non-null result, or the result could not be converted to the specified type <br/>
    /// When <see cref="UnionContainerOptions.DefaultAsNull"/> is true this method will also return a boolean if the result is the default value of the specified type <br/>
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    internal UnionContainer<TResult,bool> TryObtainResult<TResult>()
    {
        TResult? value = default;
        bool shortCircuit = ((TContainer)this).HasResult() is false;
        try
        {
            value = TryGetValue<TResult>();
        }
        catch
        {
            shortCircuit = true;
        }
        if (shortCircuit is false && value.IsNull())
        {
            shortCircuit = true;
        }
        if(shortCircuit is false && value.IsDefault() && UnionContainerOptionsInternal.DefaultAsNull)
        {
            shortCircuit = true;
        }
        return shortCircuit ? shortCircuit : value!;
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
    
    private TContainer StoreMatchedItem<T>(T value)
    {
        if(value is null || MatchedThisCycle)
        {
            return (TContainer)this;
        }
        ItemMatchedThisCycle = value;
        MatchedThisCycle = true;
        return (TContainer)this;
    }
}


