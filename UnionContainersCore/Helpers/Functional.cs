using System.Runtime.CompilerServices;
using UnionContainers.Core.Common;
using UnionContainers.Shared.Common;

namespace UnionContainers.Core.Helpers;

public static partial class Functional
{
    
    /// <summary>
    /// Executes an function on an item if it is not null, otherwise does nothing, returns the result
    /// </summary>
    /// <param name="item"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    public static TResult? IfNotNullDo<T,TResult>(this T? item, Func<T?,TResult> action, TResult fallback = default)
        => item is not null ? action.TryCatch(item) : fallback;

    /// <summary>
    /// Executes an function on an item if it is not null, otherwise does nothing, returns the original item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    public static T? IfNotNullDo<T>(this T? item, Action<T> action) => item is not null ? action.TryCatch(item).ContinueWith(item) : item; 

   
    /// <summary>
    /// Executes an action to an item if it is not null, otherwise does nothing, does not return
    /// </summary>
    /// <param name="item"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Empty IfNotNullDo<T>(this T? item, Action action)
        => item is not null ? action.TryCatch() : Empty.Nothing;
    
    
    /// <summary>
    /// Executes an action if the item is null, otherwise does nothing, does not return
    /// </summary>
    /// <param name="item"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Empty IfNullDo<T>(this T? item, Action action)
        => item is null ? action.TryCatch() : Empty.Nothing;
    
    /// <summary>
    /// Executes an action if the item is null, otherwise does nothing, does not return
    /// </summary>
    /// <param name="item"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Empty IfNullDo<T>(this T? item, Action<T?> action)
        => item is null ? action.TryCatch(item) : Empty.Nothing;
    
    /// <summary>
    /// Executes an action if the item is null, otherwise does nothing, returns the item for chaining
    /// </summary>
    /// <param name="item"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T IfNullDo<T>(this T? item, Func<T> action)
        => item is null ? action.TryCatch() : item;


    /// <summary>
    /// Applies an action to each item in a sequence if it is not null, otherwise does nothing, does not return
    /// </summary>
    /// <param name="items"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Empty ForEachIfNotNull<T>(this IEnumerable<T?> items, Action<T> action)
    {
        foreach (var item in items)
        {
            item.IfNotNullDo(action);
        }
        return Empty.Nothing;
    }
    
    
    /// <summary>
    /// Executes the provided method, depending on if the supplied predicate is true or false
    /// </summary>
    /// <param name="item"></param>
    /// <param name="condition"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static TResult? DoIf<T,TResult>(this T? item, Predicate<T> condition, Func<T, TResult> action) where T : class where TResult : class
        => condition(item) ? action.TryCatch(item) : default;
    
    
    /// <summary>
    /// Passes the item to the condition and returns the result
    /// </summary>
    /// <param name="item"></param>
    /// <param name="condition"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool CheckIf<T>(this T? item, Predicate<T> condition) => condition(item);


    /// <summary>
    /// Executes the provided predicate, returning true or false
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static bool CheckIf(BooleanExpression condition) => condition.Evaluate();
    
    
    /// <summary>
    /// if the condition is true, the item is passed to the action and executed
    /// otherwise returns default
    /// </summary>
    /// <param name="checkResult"></param>
    /// <param name="item"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static TResult? ThenDo<T,TResult>(this bool checkResult, T item, Func<T, TResult> action) where T : class where TResult : class
        => checkResult ? action.TryCatch(item) : default;
    
    /// <summary>
    /// Executes the action if the checkResult is true, otherwise returns Empty.Nothing and does nothing
    /// </summary>
    /// <param name="checkResult"></param>
    /// <param name="item"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Empty ThenDo<T>(this bool checkResult,T item, Action<T> action)
        => checkResult ? action.TryCatch(item).ContinueWith(Empty.Nothing) : Empty.Nothing;
    
    /// <summary>
    /// Executes the doAction if the checkResult is true, otherwise executes the elseAction
    /// </summary>
    /// <param name="checkResult"></param>
    /// <param name="item"></param>
    /// <param name="doAction"></param>
    /// <param name="elseAction"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Empty ThenDo<T>(this bool checkResult,T item, Action<T> doAction, Action<T> elseAction)
        => checkResult ? doAction.TryCatch(item).ContinueWith(Empty.Nothing) : elseAction.TryCatch(item).ContinueWith(Empty.Nothing);
    
   
    /// <summary>
    /// Executes the action if the checkResult is true, otherwise returns Empty.Nothing and does nothing
    /// </summary>
    /// <param name="checkResult"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static bool ThenDo(this bool checkResult, Action action)
        => checkResult ? action.TryCatch().ContinueWith(checkResult) : checkResult;
    
    /// <summary>
    /// Executes the doAction if the checkResult is true, otherwise executes the elseAction
    /// </summary>
    /// <param name="checkResult"></param>
    /// <param name="doAction"></param>
    /// <param name="elseAction"></param>
    /// <returns></returns>
    public static Empty ThenDo(this bool checkResult, Action doAction, Action elseAction)
        => checkResult ? doAction.TryCatch().ContinueWith(Empty.Nothing) : elseAction.TryCatch().ContinueWith(Empty.Nothing);
    
    public static bool ElseDo(this bool checkResult, Action action)
        => checkResult ? checkResult : action.TryCatch().ContinueWith(checkResult);
    
    /// <summary>
    /// Executes the supplied delegate with the provided arguments, returns Empty.Nothing if successful <br/>
    /// An optional method can be provided to handle exceptions, if not provided the exception will be thrown and is expected to be handled by the caller
    /// </summary>
    /// <param name="action"></param>
    /// <param name="args"></param>
    /// <typeparam name="THandle"></typeparam>
    /// <returns></returns>
    public static Empty TryCatch(this Action action, Action<Exception>? catchHandler = null)
    {
        try
        {
            action();
            return Empty.Nothing;
        }
        catch (Exception e)
        {
            if(catchHandler is null)
            {
                throw;
            }
            catchHandler(e);
            return Empty.Nothing;
        }
    }
    
    /// <summary>
    /// Executes the supplied delegate with the provided arguments, returns Empty.Nothing if successful <br/>
    /// An optional method can be provided to handle exceptions, if not provided the exception will be thrown and is expected to be handled by the caller
    /// </summary>
    /// <param name="action"></param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Empty TryCatch<T>(this Action<T> action, T item, Action<Exception>? catchHandler = null)
    {
        try
        {
            action(item);
            return Empty.Nothing;
        }
        catch (Exception e)
        {
            if(catchHandler is null)
            {
                throw;
            }
            catchHandler(e);
            return Empty.Nothing;
        }
    }
    
    /// <summary>
    /// Executes the supplied delegate with the provided arguments, returns the result if successful <br/>
    /// An optional method can be provided to handle exceptions, if not provided the exception will be thrown and is expected to be handled by the caller
    /// </summary>
    /// <param name="func"></param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static TResult? TryCatch<T, TResult>(this Func<T?, TResult?> func, T? item, Func<Exception,TResult>? catchHandler = null)
    {
        try
        {
            return func(item);
        }
        catch (Exception e)
        {
            if(catchHandler is null)
            {
                throw;
            }
            return catchHandler(e);
        }
    }
    
    /// <summary>
    /// Executes the supplied delegate with the provided arguments, returns the result if successful <br/>
    /// An optional method can be provided to handle exceptions, if not provided the exception will be thrown and is expected to be handled by the caller
    /// </summary>
    /// <param name="func"></param>
    /// <param name="catchHandler"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static TResult TryCatch<TResult>(this Func<TResult> func, Func<Exception,TResult>? catchHandler = null)
    {
        try
        {
            return func();
        }
        catch (Exception e)
        {
            if(catchHandler is null)
            {
                throw;
            }
            return catchHandler(e);
        }
    }
    
    /// <summary>
    /// Executes the supplied delegate with the provided arguments, returns the result if successful <br/>
    /// An optional method can be provided to handle exceptions, if not provided the exception will be thrown and is expected to be handled by the caller
    /// </summary>
    /// <param name="func"></param>
    /// <param name="item"></param>
    /// <param name="catchHandler"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<TResult?> TryCatch<T, TResult>(this Func<T, Task<TResult>> func, T item, Action<Exception>? catchHandler = null)
    {
        try
        {
            return await func(item);
        }
        catch (Exception e)
        {
            if(catchHandler is null)
            {
                throw;
            }
            catchHandler(e);
            return default;
        }
    }
    
    /// <summary>
    /// Executes & awaits the supplied task with the provided arguments, returns the result if successful <br/>
    /// An optional method can be provided to handle exceptions, if not provided the exception will be thrown and is expected to be handled by the caller
    /// </summary>
    /// <param name="func"></param>
    /// <param name="catchHandler"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<TResult?> TryCatch<TResult>(this Task<TResult> func, Action<Exception>? catchHandler = null)
    {
        try
        {
            return await func;
        }
        catch (Exception e)
        {
            if(catchHandler is null)
            {
                throw;
            }
            catchHandler(e);
            return default;
        }
    }
    
    
    /// <summary>
    /// Executes & awaits the supplied task with the provided arguments <br/>
    /// An optional method can be provided to handle exceptions, if not provided the exception will be thrown and is expected to be handled by the caller
    /// </summary>
    /// <param name="task"></param>
    /// <param name="catchHandler"></param>
    /// <returns></returns>
    public static async Task TryCatch(this Task task, Action<Exception>? catchHandler = null)
    {
        try
        {
            await task;
        }
        catch (Exception e)
        {
            if(catchHandler is null)
            {
                throw;
            }
            catchHandler(e);
        }
    }
    
    
    /// <summary>
    /// Used to allow chaining of functions and still return whatever item is relevant in the call chain
    /// </summary>
    /// <param name="appliedItem">Can be anything allowing for robust method chaining</param>
    /// <param name="itemToReturn">The item that is needed to end or continue the chain correctly</param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    /// <returns></returns>
    public static TReturn ContinueWith<T,TReturn> (this T? appliedItem, TReturn itemToReturn) => itemToReturn;
    
    public static TReturn? ContinueWith<T,TReturn> (this T? appliedItem) => default;
    
    
    
    /// <summary>
    /// Returns true if the item is the default value for the type
    /// </summary>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsDefault<T>(this T? item) => EqualityComparer<T>.Default.Equals(item, default);
    


    
    /// <summary>
    /// Returns true if the item is null or the default value for the type
    /// </summary>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsNullOrDefault<T>(this T? item) => item.IsNull() || item.IsDefault();
    
    /// <summary>
    /// Throws an exception if the supplied item is null
    /// Will include the method name, file path, line number, and a custom message
    /// </summary>
    /// <param name="value"></param>
    /// <param name="memberName"></param>
    /// <param name="sourceFilePath"></param>
    /// <param name="sourceLineNumber"></param>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static T ThrowIfNull<T>(this T? value, 
        [CallerMemberName] string memberName = "", 
        [CallerFilePath] string sourceFilePath = "", 
        [CallerLineNumber] int sourceLineNumber = 0, 
        [CallerArgumentExpression(nameof(value))] string? message = "") 
        => value ?? throw new NullReferenceException($"Object {message} is null, \n\t type is: {typeof(T)}, \n\t method: {memberName}, \n\t file: {sourceFilePath}, \n\t line: {sourceLineNumber}");
}