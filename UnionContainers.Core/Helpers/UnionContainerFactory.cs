using System.Runtime.CompilerServices;
using HelpfulTypesAndExtensions;

namespace UnionContainers;

public class UnionContainerFactory
{
    /// <summary>
    /// Creates an empty <see cref="UnionContainer{TValue}"/>. <br/>
    /// Can be used to represent when there is no result, error, or exception. <br/>
    /// An example could be successfully executing a method that returns void, or a successful operation that did not return a value like finding a user by ID in a database.
    /// </summary>
    /// <typeparam name="TValue">The type of the value that the container can hold.</typeparam>
    /// <returns>An empty <see cref="UnionContainer{TValue}"/>.</returns>
    public static UnionContainer<TValue> CreateEmptyContainer<TValue>() => new();
    
    /// <summary>
    /// Creates a <see cref="UnionContainer{TValue}"/> with an error.
    /// </summary>
    /// <typeparam name="TValue">The type of the value that the container can hold.</typeparam>
    /// <param name="error">The error to add to the container.</param>
    /// <returns>A <see cref="UnionContainer{TValue}"/> containing the error.</returns>
    public static UnionContainer<TValue> CreateErrorContainer<TValue>(IError error) => new(error);
    
    /// <summary>
    /// Creates a <see cref="UnionContainer{TValue}"/> with an exception.
    /// </summary>
    /// <typeparam name="TValue">The type of the value that the container can hold.</typeparam>
    /// <param name="exception">The exception to wrap in an error.</param>
    /// <returns>A <see cref="UnionContainer{TValue}"/> containing the exception.</returns>
    public static UnionContainer<TValue> CreateExceptionContainer<TValue>(Exception exception) => new(CustomErrors.Exception(exception));
    
    /// <summary>
    /// Creates a <see cref="UnionContainer{TValue}"/> with a result value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value that the container can hold.</typeparam>
    /// <param name="result">The result value.</param>
    /// <returns>A <see cref="UnionContainer{TValue}"/> containing the result value.</returns>
    public static UnionContainer<TValue> CreateResultContainer<TValue>(TValue result) => new(result);
}