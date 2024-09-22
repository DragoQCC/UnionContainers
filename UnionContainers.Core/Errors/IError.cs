using System;

namespace UnionContainers.Errors;


/// <summary>
/// An error that can be thrown by the application <br/>
/// Can be used in place of exceptions to provide more context and metadata about the error without needing to catch it
/// </summary>
public interface IError
{
    /// <summary>
    /// The name of the error
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// The message that describes the error
    /// </summary>
    public string? Message { get; set; }
    
    /// <summary>
    /// The DateTime that the error was created
    /// </summary>
    public DateTime CreationTime { get; set; }
    
    /// <summary>
    /// The source of the error, often used to identify the class or method that caused the error
    /// </summary>
    public string? Source { get; set; }
    
    /// <summary>
    /// The severity of the error <br/>
    /// Options: Unknown, Low, Medium, High, Critical
    /// </summary>
    public ErrorSeverity PriorityLevel { get; set; }
    
    /// <summary>
    /// The type of error that occurred <br/>
    /// Example: ValidationFailure, Timeout, NotFound, NetworkingError, Custom, etc. <br/>
    /// Unique error types can be defined by creating a new record that inherits from ErrorTypes
    /// </summary>
    public ErrorType Type { get; set; }
    
    /// <summary>
    /// Optional inner error that caused this error, often used for chaining errors 
    /// </summary>
    public IError? InnerError { get; set; }
    
    /// <summary>
    /// Arbitrary metadata that can be attached to the error
    /// </summary>
    public IDictionary<string, object>? MetaData { get; set; }
    
    public void SetMessage(string? message) => Message = message;
    
    public void SetSource(string source) => Source = source;
    
    public void SetPriorityLevel(ErrorSeverity priorityLevel) => PriorityLevel = priorityLevel;
    
    public void SetInnerError(IError innerError) => InnerError = innerError;
    
    public void SetMetaData(IDictionary<string, object> metaData) => MetaData = metaData;
    
    public void AddMetaData(string key, object value) => MetaData ??= new Dictionary<string, object> {{key, value}};
    
    public void AddMetaData(KeyValuePair<string, object> metaData) => MetaData ??= new Dictionary<string, object> {{metaData.Key, metaData.Value}};
    
    public void AddMetaData(IEnumerable<KeyValuePair<string, object>> metaData) => MetaData ??= metaData.ToDictionary(x => x.Key, x => x.Value);
    
    
    public string GetName() => Name;
    public string GetMessage() => Message ?? "No message provided";
    public DateTime GetCreationTime() => CreationTime;
    public DateTime GetCreationTimeAs(TimeZoneInfo timeZone) => TimeZoneInfo.ConvertTime(CreationTime, timeZone);
    public string GetSource() => Source ?? string.Empty;
    public ErrorSeverity GetPriorityLevel() => PriorityLevel;
    public ErrorType GetType() => Type;
    public IError? GetInnerError() => InnerError;
    public IDictionary<string, object>? GetMetaData() => MetaData;
}
