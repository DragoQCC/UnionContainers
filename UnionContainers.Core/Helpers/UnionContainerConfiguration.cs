using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace UnionContainers.Helpers;

public static class UnionContainerConfigurationExtensions
{
    public static IServiceCollection AddUnionContainerConfiguration(this IServiceCollection services, Action<UnionContainerOptions>? options = null)
    {
        services.AddSingleton(new UnionContainerConfiguration(options));
        return services;
    }
}



public class UnionContainerConfiguration
{
    internal static UnionContainerOptions UnionContainerOptionsInternal { get;  set; } = new();
    public UnionContainerOptions UnionContainerOptions { get;  private set; }
    
    public UnionContainerConfiguration(Action<UnionContainerOptions>? options = null)
    {
        UnionContainerOptions = new();
        options?.Invoke(UnionContainerOptions);
        UnionContainerOptionsInternal = UnionContainerOptions;
    }
}

public class UnionContainerOptions
{
    /// <summary>
    /// When true the default value will be treated as null <br/>
    /// This means container processing methods will not count the default value as a value and will be treated as empty instead <br/>
    /// Defaults to true
    /// </summary>
    public bool DefaultAsNull { get; private set; } = true;


    /// <summary>
    /// Default behavior is to set container to empty until a value is set
    /// if this is set to true the container will not be empty if there are errors or exceptions as well
    /// Defaults to true
    /// </summary>
    public bool ContainersNotEmptyIfIssues { get; private set; } = true;


    /// <summary>
    /// When true internal exceptions that happen during container processing will be treated as errors <br/>
    /// If treated as errors they are added to the containers error state as strings <br/>
    /// Otherwise exceptions will be thrown if produced from user errors <br/>
    /// Defaults to false
    /// </summary>
    public bool TreatExceptionsAsErrors { get; private set; } = false;


    /// <summary>
    /// If true will throw exceptions that happen during processing methods passed to the container <br/>
    /// example: <br/>
    /// When true:
    /// <code>
    /// // throws exception
    /// UnionContainer{string} container = MethodToContainer(() => HttpClient.GetStringAsync("https://www.throwexceptionpls.com"));
    /// </code>
    /// when false:
    /// <code>
    /// // does not throw exception, instead exceptions are added to the container error state or suppressed depending on <see cref="TreatExceptionsAsErrors"/>
    /// UnionContainer{string} container = MethodToContainer(() => HttpClient.GetStringAsync("https://www.throwexceptionpls.com"));
    /// </code>
    /// Defaults to false
    /// </summary>
    public bool ThrowExceptionsFromUserHandlingCode { get;  private set; } = false;

    public ContainerLoggingOptions LoggingOptions { get; private set; } = new();
    

    
    public UnionContainerOptions SetDefaultAsNull(bool treatDefaultAsNull)
    {
        DefaultAsNull = treatDefaultAsNull;
        UnionContainerConfiguration.UnionContainerOptionsInternal.DefaultAsNull = treatDefaultAsNull;
        return this;
    }
    
    public UnionContainerOptions SetContainersNotEmptyIfIssues(bool containersNotEmptyIfIssues)
    {
        ContainersNotEmptyIfIssues = containersNotEmptyIfIssues;
        UnionContainerConfiguration.UnionContainerOptionsInternal.ContainersNotEmptyIfIssues = containersNotEmptyIfIssues;
        return this;
    }
    
    public UnionContainerOptions SetTreatExceptionsAsErrors(bool treatExceptionsAsErrors)
    {
        TreatExceptionsAsErrors = treatExceptionsAsErrors;
        UnionContainerConfiguration.UnionContainerOptionsInternal.TreatExceptionsAsErrors = treatExceptionsAsErrors;
        return this;
    }
    
    public UnionContainerOptions SetThrowExceptionsFromUserHandlingCode(bool throwExceptionsFromUserHandlingCode)
    {
        ThrowExceptionsFromUserHandlingCode = throwExceptionsFromUserHandlingCode;
        UnionContainerConfiguration.UnionContainerOptionsInternal.ThrowExceptionsFromUserHandlingCode = throwExceptionsFromUserHandlingCode;
        return this;
    }
    
    public UnionContainerOptions SetLoggerOptions(Action<ContainerLoggingOptions> loggerOptions)
    {
        loggerOptions(LoggingOptions);
        return this;
    }
    
}


public class ContainerLoggingOptions
{
    public ILogger Logger { get; private set; } = NullLoggerFactory.Instance.CreateLogger<UnionContainerConfiguration>();
    public ContainerCreationLogging ContainerCreationLogging { get; private set; } = new(true, LogLevel.Error);
    public ContainerConversionLogging ContainerConversionLogging { get; private set; } = new(true, LogLevel.Error);
    public ContainerModificationLogging ContainerModificationLogging { get; private set; } = new(true, LogLevel.Error);
    public ContainerResultHandlingLogging ContainerResultHandlingLogging { get; private set; } = new(true, LogLevel.Error);
    public ContainerErrorHandlingLogging ContainerErrorHandlingLogging { get; private set; } = new(true, LogLevel.Error);
    public ContainerErrorHandlingLogging ContainerExceptionHandlingLogging { get; private set; } = new(true, LogLevel.Error);
    
    public ContainerLoggingOptions SetLogger(ILogger logger)
    {
        Logger = logger;
        UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.Logger = logger;
        return this;
    }
    
    /// <summary>
    /// Sets the logging options for container creation <br/>
    /// Defaults to true, LogLevel.Error <br/>
    /// Used for any container creation logging that could fail for example
    /// <c>
    /// await MethodToContainer{UnionContainer{HttpResponseMessage}}(async () => await TryConnectAsync("localhost", "http", 5005));
    /// </c>
    /// The `MethodToContainer` method could encounter an exception that prevents it from creating the container
    /// </summary>
    /// <param name="enabled"></param>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    public ContainerLoggingOptions SetContainerCreationLogging(bool enabled, LogLevel logLevel)
    {
        ContainerCreationLogging = new(enabled, logLevel);
        UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerCreationLogging = new(enabled, logLevel);
        return this;
    }
    
    public ContainerLoggingOptions SetContainerConversionLogging(bool enabled, LogLevel logLevel)
    {
        ContainerConversionLogging = new(enabled, logLevel);
        UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerConversionLogging = new(enabled, logLevel);
        return this;
    }
    
    public ContainerLoggingOptions SetContainerModificationLogging(bool enabled, LogLevel logLevel)
    {
        ContainerModificationLogging = new(enabled, logLevel);
        UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerModificationLogging = new(enabled, logLevel);
        return this;
    }
    
    public ContainerLoggingOptions SetContainerResultHandlingLogging(bool enabled, LogLevel logLevel)
    {
        ContainerResultHandlingLogging = new(enabled, logLevel);
        UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerResultHandlingLogging = new(enabled, logLevel);
        return this;
    }
    
    public ContainerLoggingOptions SetContainerErrorHandlingLogging(bool enabled, LogLevel logLevel)
    {
        ContainerErrorHandlingLogging = new(enabled, logLevel);
        UnionContainerConfiguration.UnionContainerOptionsInternal.LoggingOptions.ContainerErrorHandlingLogging = new(enabled, logLevel);
        return this;
    }
    
    internal ContainerLoggingOptions() {}
}

