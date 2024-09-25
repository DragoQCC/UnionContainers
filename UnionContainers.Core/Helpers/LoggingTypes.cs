using Microsoft.Extensions.Logging;

namespace UnionContainers;

public sealed record ContainerCreationLogging(bool Log, LogLevel LogLevel);
public sealed record ContainerConversionLogging(bool Log, LogLevel LogLevel);
public sealed record ContainerModificationLogging(bool Log, LogLevel LogLevel);
public sealed record ContainerResultHandlingLogging(bool Log, LogLevel LogLevel);
public sealed record ContainerErrorHandlingLogging(bool Log, LogLevel LogLevel);
