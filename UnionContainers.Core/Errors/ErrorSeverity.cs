namespace UnionContainers.Errors;

/// <summary>
/// The severity of the error <br/>
/// Determines how critical the error is <br/>
/// Default is Medium
/// </summary>
public enum ErrorSeverity
{
    /// <summary>
    /// Set when the severity of the error is unknown
    /// </summary>
    Unknown = -2,
    /// <summary>
    /// A low severity error, can be considered informational
    /// </summary>
    Low = -1,
    /// <summary>
    /// The baseline error level <br/> A medium severity error, can be considered a warning
    /// </summary>
    Medium = 0,
    /// <summary>
    /// A high severity error, can indicate a problem that needs to be addressed
    /// </summary>
    High = 1,
    /// <summary>
    /// A critical severity error, indicates a problem that needs to be addressed immediately / a fatal error
    /// </summary>
    Critical = 2
}