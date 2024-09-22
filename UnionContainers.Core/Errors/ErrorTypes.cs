using HelpfulTypesAndExtensions;
using UnionContainers.Helpers;
using UnionContainers.Shared.Common;

namespace UnionContainers.Errors;



public interface IClientError<T> : IError where T : struct, IClientError<T>;

public static class ClientErrors
{
    public static ValidationFailureError ValidationFailure(string? message = null) => new ValidationFailureError().SetMessage(message);
    public static BadRequestError BadRequest(string? message = null) => new BadRequestError().SetMessage(message);
    public static NotFoundError NotFound(string? message = null) => new NotFoundError().SetMessage(message);
    public static UnauthorizedError Unauthorized(string? message = null) => new UnauthorizedError().SetMessage(message);  
    public static ForbiddenError Forbidden(string? message = null) => new ForbiddenError().SetMessage(message);
    public static MissingAuthenticationError MissingAuthentication(string? message = null) => new MissingAuthenticationError().SetMessage(message);
    public static InvalidOperationError InvalidOperation(string? message = null) => new InvalidOperationError().SetMessage(message);
    
    public record struct ValidationFailureError() : IClientError<ValidationFailureError>
    {
        /// <inheritdoc />
        public string Name { get; set; } = "Validation Failure";

        /// <inheritdoc />
        public string? Message { get; set; } = "A validation error occurred";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.ValidationFailure;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null; 
    }
    
    public record struct BadRequestError() : IClientError<BadRequestError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Bad Request";
        
        /// <inheritdoc />
        public string? Message { get; set; } = "The request was malformed or invalid";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.BadRequest;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct NotFoundError() : IClientError<NotFoundError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Not Found"; 
        
        /// <inheritdoc />
        public string? Message { get; set; } = "The requested resource was not found";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.NotFound;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct UnauthorizedError() : IClientError<UnauthorizedError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Unauthorized";
        
        /// <inheritdoc />
        public string? Message { get; set; } = "The request was unauthorized";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.Unauthorized;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct ForbiddenError() : IClientError<ForbiddenError>
    {
        /// <inheritdoc />
        public string Name { get; }  = "Forbidden";
        
        /// <inheritdoc />
        public string? Message { get; set; } = "The request was forbidden";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.Forbidden;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct MissingAuthenticationError() : IClientError<MissingAuthenticationError>
    {
        /// <inheritdoc />
        public string Name { get; }  = "Missing Authentication";
        
        /// <inheritdoc />
        public string? Message { get; set; } = "The request was missing authentication";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.MissingAuthentication;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct InvalidOperationError() : IClientError<InvalidOperationError>
    {
        /// <inheritdoc />
        public string Name { get; } =  "Invalid Operation";
        
        /// <inheritdoc />
        public string? Message { get; set; } = "An invalid operation was attempted";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.InvalidOperation;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
}


public interface IServerError<T> : IError where T : struct, IServerError<T>;

/// <summary>
/// Includes
/// GenericFailure,  
///Unexpected,  
///ServiceUnavailable,  
///CircuitBreaker,  
///DependencyFailure,  
///DataIntegrity,  
///PreconditionFailed  
/// </summary>
public static class ServerErrors
{
    public static GenericFailureError GenericFailure(string? message = null) => new GenericFailureError().SetMessage(message);
    public static UnexpectedError Unexpected(string? message = null) => new UnexpectedError().SetMessage(message);
    public static ServiceUnavailableError ServiceUnavailable(string? message = null) => new ServiceUnavailableError().SetMessage(message);
    public static CircuitBreakerError CircuitBreaker(string? message = null) => new CircuitBreakerError().SetMessage(message);
    public static DependencyFailureError DependencyFailure(string? message = null) => new DependencyFailureError().SetMessage(message);
    public static DataIntegrityError DataIntegrity(string? message = null) => new DataIntegrityError().SetMessage(message);
    public static PreconditionFailedError PreconditionFailed(string? message = null) => new PreconditionFailedError().SetMessage(message);
    
    public record struct GenericFailureError() : IServerError<GenericFailureError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Generic Failure";
        
        /// <inheritdoc />
        public string? Message { get; set; } = "A generic failure occurred";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.GenericFailure;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }

    public record struct UnexpectedError() : IServerError<UnexpectedError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Unexpected Error";


        /// <inheritdoc />
        public string? Message { get; set; } = "An unexpected error occurred";


        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;


        /// <inheritdoc />
        public string? Source { get; set; } = "";


        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;


        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.Unexpected;


        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;


        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct ServiceUnavailableError() : IServerError<ServiceUnavailableError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Service Unavailable";
        
        /// <inheritdoc />
        public string? Message { get; set; } = "The service is currently unavailable";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.ServiceUnavailable;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct CircuitBreakerError() : IServerError<CircuitBreakerError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Circuit Breaker";
        
        /// <inheritdoc />
        public string? Message { get; set; } = "The circuit breaker has been tripped";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.CircuitBreaker;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct DependencyFailureError() : IServerError<DependencyFailureError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Dependency Failure";
        
        /// <inheritdoc />
        public string? Message { get; set; } = "A dependency failure occurred";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.DependencyFailure;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct DataIntegrityError() : IServerError<DataIntegrityError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Data Integrity";
        
        /// <inheritdoc />
        public string? Message { get; set; } = "A data integrity error occurred";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.DataIntegrity;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct PreconditionFailedError() : IServerError<PreconditionFailedError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Precondition Failed";
        
        /// <inheritdoc />
        public string? Message { get; set; } = "A precondition failed error occurred";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.PreconditionFailed;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }

}


public interface INetworkError<T> : IError where T : struct, INetworkError<T>;

/// <summary>
/// Includes
/// NetworkingError,
/// RateLimit,
/// Timeout,
/// ConnectionFailure,
/// AuthenticationFailure
/// </summary>
public static class NetworkErrors
{
    public static NetworkingError GenericNetworking(string? message = null) => new NetworkingError().SetMessage(message);
    public static RateLimitError RateLimit(string? message = null) => new RateLimitError().SetMessage(message);
    public static TimeoutError Timeout(string? message = null) => new TimeoutError().SetMessage(message);
    public static ConnectionFailureError ConnectionFailure(string? message = null) => new ConnectionFailureError().SetMessage(message);
    public static AuthenticationFailureError AuthenticationFailure(string? message = null) => new AuthenticationFailureError().SetMessage(message);
    
    public record struct NetworkingError() : INetworkError<NetworkingError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Networking Error";
        
        /// <inheritdoc />
        public string? Message { get; set; } = "A networking error occurred";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.NetworkingError;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct RateLimitError() : INetworkError<RateLimitError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Rate Limit";
        
        /// <inheritdoc />
        public string? Message { get; set; } = "The rate limit has been exceeded";
        
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.RateLimit;
        
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }

    public record struct TimeoutError() : INetworkError<TimeoutError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Timeout";
        /// <inheritdoc />
        public string? Message { get; set; } = "The request timed out";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.Timeout;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct ConnectionFailureError() : INetworkError<ConnectionFailureError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Connection Failure";
        /// <inheritdoc />
        public string? Message { get; set; } = "A connection failure occurred";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.NetworkingError;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct AuthenticationFailureError() : INetworkError<AuthenticationFailureError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Authentication Failure";
        /// <inheritdoc />
        public string? Message { get; set; } = "An authentication failure occurred";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.NetworkingError;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
}


public interface ISecurityError<T> : IError where T : struct, ISecurityError<T>;

/// <summary>
/// Includes
/// Unauthorized,
/// MissingAuthentication,
/// Forbidden
/// </summary>
public static class SecurityErrors 
{
    public static UnauthorizedError Unauthorized(string? message = null) => new UnauthorizedError().SetMessage(message);
    public static MissingAuthenticationError MissingAuthentication(string? message = null) => new MissingAuthenticationError().SetMessage(message);
    public static ForbiddenError Forbidden(string? message = null) => new ForbiddenError().SetMessage(message);
    
    public record struct UnauthorizedError() : ISecurityError<UnauthorizedError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Unauthorized";
        /// <inheritdoc />
        public string? Message { get; set; } = "The request was unauthorized";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.Unauthorized;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct MissingAuthenticationError() : ISecurityError<MissingAuthenticationError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Missing Authentication";
        /// <inheritdoc />
        public string? Message { get; set; } = "The request was missing authentication";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.MissingAuthentication;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }

    public record struct ForbiddenError() : ISecurityError<ForbiddenError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Forbidden";
        /// <inheritdoc />
        public string? Message { get; set; } = "The request was forbidden";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.Forbidden;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
}


public interface IResourceError<T> : IError where T : struct, IResourceError<T>;

/// <summary>
/// Includes,
/// NotFound
/// ServiceUnavailable
/// RateLimit
/// CircuitBreaker
/// DependencyFailure
/// </summary>
public static class ResourceErrors 
{
    public static NotFoundError NotFound(string? message = null) => new NotFoundError().SetMessage(message);
    public static ServiceUnavailableError ServiceUnavailable(string? message = null) => new ServiceUnavailableError().SetMessage(message);
    public static RateLimitError RateLimit(string? message = null) => new RateLimitError().SetMessage(message);
    public static CircuitBreakerError CircuitBreaker(string? message = null) => new CircuitBreakerError().SetMessage(message);
    public static DependencyFailureError DependencyFailure(string? message = null) => new DependencyFailureError().SetMessage(message);
    
    public record struct NotFoundError() : IResourceError<NotFoundError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Not Found";
        /// <inheritdoc />
        public string? Message { get; set; } = "The requested resource was not found";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.NotFound;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct ServiceUnavailableError() : IResourceError<ServiceUnavailableError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Service Unavailable";
        /// <inheritdoc />
        public string? Message { get; set; } = "The service is currently unavailable";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.ServiceUnavailable;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }

    public record struct RateLimitError() : IResourceError<RateLimitError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Rate Limit";
        /// <inheritdoc />
        public string? Message { get; set; } = "The rate limit has been exceeded";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.RateLimit;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }

    public record struct CircuitBreakerError() : IResourceError<CircuitBreakerError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Circuit Breaker";
        /// <inheritdoc />
        public string? Message { get; set; } = "The circuit breaker has been tripped";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.CircuitBreaker;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct DependencyFailureError() : IResourceError<DependencyFailureError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Dependency Failure";
        /// <inheritdoc />
        public string? Message { get; set; } = "A dependency failure occurred";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.DependencyFailure;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
}


public interface ICustomError<T> : IError where T : struct, ICustomError<T>;

/// <summary>
/// Includes,
/// Custom,
/// Failure
/// </summary>
public static class CustomErrors
{
    public static CustomError Custom(string? message = null) => new CustomError().SetMessage(message);
    public static FailureError Failure(string? message = null) => new FailureError().SetMessage(message);
    public static GenericError Generic(string? message = null) => new GenericError().SetMessage(message);
    public static ExceptionWrapperError Exception(Exception exception, string? message = null) => new ExceptionWrapperError(exception, message);
    
    public record struct CustomError() : ICustomError<CustomError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Custom";
        /// <inheritdoc />
        public string? Message { get; set; } = "A custom error occurred";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.Custom;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct FailureError() : ICustomError<FailureError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Failure";
        /// <inheritdoc />
        public string? Message { get; set; } = "A failure occurred";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.GenericFailure;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct GenericError() : ICustomError<GenericError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Generic Error";
        /// <inheritdoc />
        public string? Message { get; set; } = "A generic error occurred";
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.Medium;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.GenericFailure;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }
    
    public record struct ExceptionWrapperError(Exception exception, string? message = null) : ICustomError<ExceptionWrapperError>
    {
        /// <inheritdoc />
        public string Name { get; } = "Exception Wrapper";
        /// <inheritdoc />
        public string? Message { get; set; } = message ?? exception.Message;
        /// <inheritdoc />
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        /// <inheritdoc />
        public string? Source { get; set; } = exception.Source ?? "";
        /// <inheritdoc />
        public ErrorSeverity PriorityLevel { get; set; } = ErrorSeverity.High;
        /// <inheritdoc />
        public ErrorType Type { get; set; } = ErrorType.Custom;
        /// <inheritdoc />
        public IError? InnerError { get; set; } = null;
        /// <inheritdoc />
        public IDictionary<string, object>? MetaData { get; set; } = null;
    }

}


public partial record struct ErrorType : IEnumeration<ErrorType>
{
    public static Enumeration<ErrorType> Types { get; internal set; } = new Enumeration<ErrorType>();
    
    /// <inheritdoc />
    public int Value { get; }


    /// <inheritdoc />
    public string DisplayName { get; }
    
    public ErrorType(int value, string displayName)
    {
        Value = value;
        DisplayName = displayName;
    }
    
    public void Switch(ErrorType targetType, Action action)
    {
        if (this == targetType)
        {
            action();
        }
    }
    
    public void Switch(Action? defaultAction = null, params ValueTuple<ErrorType,Action>[] targetType)
    {
        foreach (var (type, action) in targetType)
        {
            if (this == type)
            {
                action();
            }
            else
            {
                defaultAction?.Invoke();
            }
        }
    }
    
    public static ErrorType Custom => new ErrorType(0, "Custom");
    public static ErrorType Unexpected => new ErrorType(1, "Unexpected");
    public static ErrorType GenericFailure => new ErrorType(2, "Generic Failure");
    public static ErrorType ValidationFailure => new ErrorType(3, "Validation Failure");
    public static ErrorType NotFound => new ErrorType(4, "Not Found");
    public static ErrorType Unauthorized => new ErrorType(5, "Unauthorized");
    public static ErrorType MissingAuthentication => new ErrorType(6, "Missing Authentication");
    public static ErrorType Forbidden => new ErrorType(7, "Forbidden");
    public static ErrorType Timeout => new ErrorType(8, "Timeout");
    public static ErrorType RateLimit => new ErrorType(9, "Rate Limit");
    public static ErrorType ServiceUnavailable => new ErrorType(10, "Service Unavailable");
    public static ErrorType BadRequest => new ErrorType(11, "Bad Request");
    public static ErrorType NetworkingError => new ErrorType(12, "Networking Error");
    public static ErrorType InvalidOperation => new ErrorType(13, "Invalid Operation");
    public static ErrorType DependencyFailure => new ErrorType(14, "Dependency Failure");
    public static ErrorType DataIntegrity => new ErrorType(15, "Data Integrity");
    public static ErrorType PreconditionFailed => new ErrorType(16, "Precondition Failed");
    public static ErrorType CircuitBreaker => new ErrorType(17, "Circuit Breaker");
    
}

