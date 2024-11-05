using System.Runtime.CompilerServices;
using HelpfulTypesAndExtensions;

namespace UnionContainers;

public interface IUnionContainer
{
    /// <summary>
    /// Gets or sets the current state of the union container.
    /// </summary>
    /// <remarks>
    /// The state reflects the current status of the container, indicating whether it is empty,
    /// contains a result, encountered an error, or an exception.
    /// </remarks>
    public UnionContainerState State { get; internal set; }
    internal List<IError>? Errors { get; set; }


    /// <summary>
    /// Adds a list of errors to the container
    /// </summary>
    /// <param name="errors">An array of errors to add to the container</param>
    public void AddErrors(params IError[] errors)
    {
        Errors ??= [ ];
        Errors.AddRange(errors);
        if (State != UnionContainerState.Error)
        {
            State = UnionContainerState.Error;
        }
    }

    /// <summary>
    /// Checks if the container has any errors
    /// </summary>
    /// <returns>True if there are errors in the container, otherwise false</returns>
    public bool HasErrors() => Errors?.Count > 0;

    /// <summary>
    /// Returns a list of errors from the container if any are set, otherwise returns an empty list.
    /// </summary>
    /// <returns>A list of errors.</returns>
    public List<IError> GetErrors() => Errors ?? new List<IError>();

    /// <summary>
    /// Returns a list of errors of a specified type from the container if any are set, otherwise returns an empty list.
    /// </summary>
    /// <typeparam name="TError">The type of errors to return.</typeparam>
    /// <returns>A list of errors of the specified type.</returns>
    public List<TError> GetErrors<TError>()
    {
        Errors ??= [ ];
        if (Errors.Count == 0)
        {
            return [ ];
        }

        var returnList = new List<TError>();
        foreach (IError error in Errors)
        {
            if (error is TError errorValue)
            {
                returnList.Add(errorValue);
            }
        }

        return returnList;
    }

    /// <summary>
    /// Executes the supplied action if the container is in the specified state
    /// </summary>
    /// <param name="state">The desired state to check for</param>
    /// <param name="action">The action to run if the container is in the desired state</param>
    public void ForState(UnionContainerState state, Action action)
    {
        if (State == state)
        {
            action();
        }
    }

    /// <summary>
    /// Executes a given action if the container is in the specified state.
    /// </summary>
    /// <param name="defaultAction">Optional action to execute if the container state does not match any of the target states.</param>
    /// <param name="targetState">An array of value tuples where each tuple consists of a target state and an action to execute if the container is in that state.</param>
    public void ForState(Action? defaultAction = null, params ValueTuple<UnionContainerState, Action>[] targetState)
    {
        foreach ((UnionContainerState state, Action action) in targetState)
        {
            if (State == state)
            {
                action();
            }
            else
            {
                defaultAction?.Invoke();
            }
        }
    }

    internal void SetState(UnionContainerState state) => State = state;
}

public interface IUnionContainer<TContainer> : IUnionContainer
where TContainer : IUnionContainer<TContainer>;

public interface IUnionContainerResult<TValueTuple> : IUnionContainer<IUnionContainerResult<TValueTuple>>
where TValueTuple : struct, ITuple
{
    internal TValueTuple ResultValue { get; init; }
}