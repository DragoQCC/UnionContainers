namespace UnionContainers.Core.Common;

public record struct ValueState(bool State, object? Value);
public record struct ErrorState(bool State, List<object>? ErrorItems);
public record struct ExceptionState(bool State, Exception? Exception);
public record struct EmptyState(bool State, Empty Empty);