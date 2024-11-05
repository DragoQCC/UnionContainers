# Union Container

`UnionContainers` is a library that provides a discriminated union & result like type in C#.
It can be used when you want to return multiple types from a single method, or when you want to return a result or an error from a method.

## Example
This code
```csharp
static List<string> EmployeeNames = ["John Doe", "Jane Doe", "Bob Stevens", "Sally Stevens", "Joe Stevens"];

public static void Main()
{
    try 
    {
        Demo newDemo = new Demo();
        string? result = newDemo.TryGetEmployeeByName("Jane Doe");
        if (result is not null)
        {
            Console.WriteLine($"Employee found: {result}");
        }
        else
        {
            Console.WriteLine("Employee not found");
        }    
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

public class Demo
{
    public string? TryGetEmployeeByName(string name)
    {
        try 
        {
            if (name is null || name == "")
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (EmployeeNames.Contains(name))
            {
                return name;
            }
            return null;
        }
        catch (Exception ex)
        {
            MyLogger.Log(ex.Message);
            return null;
        }
    }
}
```

It can become this 
```csharp
static List<string> EmployeeNames = ["John Doe", "Jane Doe", "Bob Stevens", "Sally Stevens", "Joe Stevens"];

public static void Main()
{
    Demo newDemo = new Demo();
    UnionContainer<string> result = newDemo.TryGetEmployeeByName("Jane Doe");
    result.Match
    (
        onResult: matchedName => Console.WriteLine($"Employee found: {matchedName}"),
        onNoResult: () => Console.WriteLine("Employee not found"),
        onErrors: errors => errors.Foreach(error => 
        {
            MyLogger.Log(error);
        }),
        onException: ex => MyLogger.Log(ex.Message)
    );   
}

public class Demo
{
    public UnionContainer<string> TryGetEmployeeByName(UnionContainer<string> name)
    {
        return name.Match
        (
            onResult: matchedName => EmployeeNames.Contains(matchedName) ? new UnionContainer<string>(matchedName) : new UnionContainer<string>(ResourceErrors.NotFound("No employee found with that name")), 
            onNoResult: () => new UnionContainer<string>(new ArgumentException("Name cannot be null or empty"))
        );
    }
}
```
This example showcases some of the strengths of the UnionContainers library 
  - Avoiding null reference exceptions 
  - Passing exceptions back up the callstack if desired without expensive try-catch throws 
  - Being able to create powerful detailed error messages when something goes wrong



## Table of Contents

- [Union Container](#union-container)
- [Example](#example)
- [Features \& Benefits](#features--benefits)
- [Installation](#installation)
- [What is a UnionContainer?](#what-is-a-unioncontainer)
- [Using Union Containers](#using-union-containers)
  - [Creating a Union Container](#creating-a-union-container)
  - [Replace Throwing Exceptions](#replace-throwing-exceptions)
  - [Replace Null Checks & Avoid Null Reference Exceptions](#replace-null-checks--avoid-null-reference-exceptions)
  - [Replace Multiple Method Overloads](#replace-multiple-method-overloads)
  - [Create Custom Error Messages](#create-custom-error-messages)
  - [Safely Wrap Method Calls and Convert to Container](#safely-wrap-method-calls-and-convert-to-container)
- [Methods](#methods)
  - [`GetState`](#getstate)
  - [`Match`](#match)
  - [`If{State}Do` Methods](#ifstatedo-methods)
  - [`Error Related` Methods](#error-related-methods)
  - [`ToContainer` Extension Methods](#tocontainer-extension-methods)
- [Automatic Exception Conversion & Error Handling Registration](#automatic-exception-conversion--error-handling-registration)
- [Performance](#performance)
- [Contributing](#contributing)
- [License](#license)


## Features & Benefits
- Replace performance heavy exception throwing with a more controlled & faster error handling mechanism.
- Avoid null reference exceptions by using the Empty state.
- Avoid the need for multiple method overloads by using a single method that can handle multiple types.
- Allow returning multiple types from a single method.
- Create unique, robust error messages that can be passed back up the call stack.
- Write functional like code to handle and react differently based on the state of the container.




## Installation

Interested in trying the library out? You can install it via NuGet Package Manager Console by running the following
command:

```bash
Install-Package UnionContainers
```

Next, add a using statement to the top of your file to use the library

```csharp
using UnionContainers;
```

## What is a UnionContainer?

A Union Container is a type that combines two useful concepts, the discriminated union and the result type.

It is similar to a discriminated union in that it can hold one of multiple types, but it is also a result type in that it can hold an error or exception value.

It can be in one of four states:

- **Empty** - The container is empty and has no value.
- **Result** - The container has a valid value of one of the possible types.
- **Error** - The container has a custom error value set.
- **Exception** - The container has an exception value set.

## Using Union Containers

### Creating a Union Container
Creating a Union Container is simple, and can be done in a few ways.
The UnionContainers library supports both explcilitly returning a container, and implicitly returning a container with implicit casting.

Explicitly creating a container
```csharp
//using a method to create a container
public static UnionContainer<int> Divide(int num1, int num2)
{
    if (num2 == 0)
    {
        return new UnionContainer<int>(new DivideByZeroException("Cannot divide by zero"));
    }
    return new UnionContainer<int>(num1 / num2);
}
//Using the UnionContainerFactory
UnionContainerFactory.Create(5);

//Using the UnionContainer constructor
UnionContainer<int,double,float> container = new UnionContainer<int,double,float>(5);
```

Implicitly creating a container
```csharp
public static UnionContainer<int> Divide(int num1, int num2)
{
    if (num2 == 0)
    {
        return new DivideByZeroException("Cannot divide by zero");
    }
    return num1 / num2;
}
```
In this example, the Divide method will return a container with the result of the division, or an exception if the second number is zero. Regardless the type is implicitly cast to a UnionContainer<int> type.


### Replace Throwing Exceptions 

The UnionContainers library can be used to replace throwing exceptions with a more controlled error handling mechanism.

Code like this 👇
```csharp
public Employee? NameVerification(string name)
{
    if (name is null)
    {
        throw new ArgumentNullException(nameof(name));
    }
    if (name.Length < 2)
    {
        throw new ArgumentException("Name is too short");
    }
    if (name.Length > 100)
    {
        throw new ArgumentException("Name is too long");
    }
    if (name.Contains(" ") is false)
    {
        throw new ArgumentException("A first and last name is required");
    }
    if (string.IsNullOrWhiteSpace(name))
    {
        throw new ArgumentException("Name cannot be empty or whitespace only");
    }
    return new Employee(name);
}
```

Can instead be written like this 👇
```csharp
public UnionContainer<Employee> NameVerification(string name)
{
    List<IError> errors = [];
    if (name.Length < 2)
    {
        errors.Add(ClientErrors.ValidationFailure("Name is too short"));
    }
    if (name.Length > 100)
    {
        errors.Add(ClientErrors.ValidationFailure("Name is too long"));
    }
    if (name.Contains(" ") is false)
    {
        errors.Add(ClientErrors.ValidationFailure("A first and last name is required"));
    }
    if (string.IsNullOrWhiteSpace(name))
    {
        errors.Add(ClientErrors.ValidationFailure("Name cannot be empty or whitespace only"));
    }
    if (errors.Count > 0)
    {
        return errors;
    }
    return new Employee(name);
}
```

In this example, the NameVerification method will return a container with the employee object if the name is valid, or a list of errors if the name is invalid.
This allows for getting back more detailed errors about what went wrong, and avoids having to get hit by expensive stack unwinds and exception throwing.


### Replace Null Checks & Avoid Null Reference Exceptions

Having to constantly check for null values can be a pain, and can lead to null reference exceptions if not done correctly.
Code written like this 👇
```csharp
public Programmer? TryGetProgrammerByName(string? name)
{
    if (name.Length < 2)
    {
        return null;
    }
    if (string.IsNullOrWhiteSpace(name))
    {
        return null;
    }
    return Employees.FirstOrDefault(e => e.Name == name);
}

public void Main()
{
    Programmer? programmer = TryGetProgrammerByName("John Doe");
    if (programmer is not null)
    {
        Console.WriteLine($"Programmer found: {programmer.Name}");
    }
    else
    {
        Console.WriteLine("Programmer not found");
    }
}
```

It Can be written like this 👇
```csharp
public UnionContainer<Programmer> TryGetProgrammerByName(string name)
{
    list<IError> errors = [];
    if (name.Length < 2)
    {
        errors.Add(ClientErrors.ValidationFailure("Name is too short"));
    }
    if (string.IsNullOrWhiteSpace(name))
    {
        errors.Add(ClientErrors.ValidationFailure("Name cannot be empty or whitespace only"));
    }
    if (errors.Count > 0)
    {
        return errors;
    }
    return Employees.FirstOrDefault(e => e.Name == name) ?? new UnionContainer<Programmer>(ResourceErrors.NotFound("No programmer found with that name"));
}

public void Main()
{
    UnionContainer<Programmer> programmerContainer = TryGetProgrammerByName("John Doe");
    programmerContainer.Match
    (
        onResult: programmer => Console.WriteLine($"Programmer found: {programmer.Name}"),
        onNoResult: () => Console.WriteLine("Programmer not found"),
        onErrors: errors => errors.ForEach(error => Console.WriteLine(error))
    );
}
```

By using Match, you not only avoid the possibility of null reference exceptions, but you also are forced to handle the case where no result is returned.

### Replace Multiple Method Overloads

Sometimes it is necessary to have multiple method overloads to handle different types of input, or to return different types of output.
This can lead to code duplication, and can make it harder to maintain and update code bases.

For example, the following methods are all overrides of the same method but with different input types.
```csharp

public void FindEmployee(string name)
{
    Employee employee = Employees.FirstOrDefault(e => e.Name == name);
}

public void FindEmployee(Guid id)
{
    Employee employee = Employees.FirstOrDefault(e => e.Id == id);
}

public void FindEmployee(int employeeNumber)
{
    Employee employee = Employees.FirstOrDefault(e => e.EmployeeNumber == employeeNumber);
}
```

This can be replaced with a single method that can handle all of these cases.
```csharp
public void FindEmployee(UnionContainer<string,Guid,int> employeeIdentifier)
{
    Employee employee = employeeIdentifier.Match
    (
        onResult: name => Employees.FirstOrDefault(e => e.Name == name),
        onResult: id => Employees.FirstOrDefault(e => e.Id == id),
        onResult: employeeNumber => Employees.FirstOrDefault(e => e.EmployeeNumber == employeeNumber),
        onNoResult: () => null
    );
}
```

The same can be done for methods that return different types of output.
These two methods 👇

```csharp
public Employee? GetEmployeeByName(string name)
{
    return Employees.FirstOrDefault(e => e.Name == name);
}

public Manager? GetManagerByName(string name)
{
    return Managers.FirstOrDefault(m => m.Name == name);
}
```
Can instead be written like this 👇
```csharp
public UnionContainer<Employee,Manager> GetEmployeeOrManagerByName(string name)
{
    return Employees.FirstOrDefault(e => e.Name == name) ?? Managers.FirstOrDefault(m => m.Name == name);
}
```


### Create Custom Error Messages
The IError type used comes from the [HelpfulTypesAndExtensions](https://github.com/DragoQCC/HelpfulTypesAndExtensions/tree/main) library.

Every IError type has at least the following properties 
```csharp
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
}
```

Specific error types also exist that contain more detailed properties related to that specific error.

For example, network related errors also contain the status code, and headers from the response.


An example of returning a container with custom errors is shown below:

```csharp

```


### Safely Wrap Method Calls and Convert to Container

The UnionContainers library contains methods in each of the UnionContainer types to allow for wrapping a method to contain exceptions that get thrown, or allow for multiple return types.

The `MethodToContainer` methods are static methods within the UnionContainer class that can be invoked to wrap a method call and return a container.

The following is a simple example of wrapping a call to `GetAsync` from the HttpClient type, and reacting in various ways depending on the result of that call. 
```csharp
string url = "http://localhost:5000/";
var httpSimpleContainer = await UnionContainer<HttpResponseMessage>.MethodToContainer(() => client.GetAsync(url));
httpSimpleContainer.Match
(
    responseMessage => Console.WriteLine($"Http Request was successful, with response message {responseMessage}"),
    () => Console.WriteLine("Get Request returned null, no exception or errors"),
    onException: async exception =>
    {
        if (exception is TaskCanceledException)
        {
            Console.WriteLine("Waiting 5 seconds before retrying request");
            await Task.Delay(TimeSpan.FromSeconds(5));
            var secondRequest = await UnionContainer<HttpResponseMessage>.MethodToContainer(() => client.GetAsync(url));
            Console.WriteLine($"Status of second request is {secondRequest.GetState()}");
        }
    }
);
```

It is also possible to create a more advanced handling of the method result before returning the container. 

For example, if we wanted to return different errors, based on the HttpStatus or exception thrown by the HttpRequest
```csharp
public static async Task<UnionContainer<HttpResponseMessage>> HttpRequestExample(string url)
{
    var client = new HttpClient();
    var requestContainer = await UnionContainer<HttpResponseMessage>.MethodToContainer(() => client.GetAsync(url));
    return requestContainer.Match
    (
        message =>
        {
            if (message.IsSuccessStatusCode)
            {
                return new UnionContainer<HttpResponseMessage>(message);
            }

            IError error = message.StatusCode switch
            {
                HttpStatusCode.BadRequest => NetworkErrors.GenericNetworking($"Request to {url} is invalid"),
                HttpStatusCode.Unauthorized => ClientErrors.Unauthorized($"Request to {url} is missing login credentials"),
                HttpStatusCode.Forbidden => ClientErrors.Forbidden($"Users request to {url} does not have sufficient access rights and was blocked"),
                HttpStatusCode.NotFound => ClientErrors.NotFound($"The requested url {url} was not found"),
                HttpStatusCode.InternalServerError => ServerErrors.GenericFailure($"The requested endpoint at {url} returned an internal error"),
                _ => NetworkErrors.GenericNetworking($"Request to {url} failed with status code {message.StatusCode} and reason {message.ReasonPhrase}")
            };

            return new UnionContainer<HttpResponseMessage>(error);
        }, 
        () => new UnionContainer<HttpResponseMessage>(CustomErrors.Custom($"The request to {url}, returned null")), 
        onException: (exception) =>
        {
            switch (exception)
            {
                case TaskCanceledException tcex : return new UnionContainer<HttpResponseMessage>(NetworkErrors.Timeout($"The request to {url} timed out: {tcex.Message}"));
                case UriFormatException uriEx : return new UnionContainer<HttpResponseMessage>(ClientErrors.ValidationFailure($"The requested URI {url} is invalid: {uriEx.Message}"));
                case HttpRequestException httpRequestEx :
                {
                    var networkingError = NetworkErrors.GenericNetworking($"The request to {url} failed with status code {httpRequestEx.StatusCode} and reason {httpRequestEx.Message}");
                    networkingError.SetSource(httpRequestEx.Source);
                    return new UnionContainer<HttpResponseMessage>(networkingError);
                }
                case InvalidOperationException invalidOperationEx : return new UnionContainer<HttpResponseMessage>(invalidOperationEx);
                default: return new UnionContainer<HttpResponseMessage>(exception);
            }
        }
    );
}
```

it may also be desirable to wrap existing methods to avoid needing to update code bases, or APIs

- Legacy method we don't want to change
```csharp
public static double MyLegacyDivideMethod(double num1, int num2)
{
    return num1 / num2;
}
```

- Previous non UnionContainer logic
```csharp
try
{
    double result = MyLegacyDivideMethod(5, 0);
    Console.WriteLine($"Result of division is {result}");
}
catch (Exception e)
{
    Console.WriteLine("No result was returned");
    if(e is DivideByZeroException)
    {
        Console.WriteLine("Cannot divide by zero, make sure the second number is not zero");
    }
    else
    {
        Console.WriteLine(e);
    }
    throw;
}
```

- Post UnionContainer logic
- Make a small wrapper method over the legacy method to return a container instead
```csharp
var doubleDivideContainer = UnionContainer<double>.MethodToContainer(() => MyLegacyDivideMethod(5, 0));
doubleDivideContainer.Match
(
    result => Console.WriteLine($"Result of division is {result}"),
    () => Console.WriteLine("No result was returned"),
    onErrors: errors => errors.ForEachIfNotNull(error => Console.WriteLine($"\t{error}"))
);

```


## Methods
The UnionContainers type has numerous methods and extension methods, and not all of them will end up in this documentation

### `GetState`

Returns the state of the container, will be one of four options:
- UnionContainerState.Empty
- UnionContainerState.Result
- UnionContainerState.Error
- UnionContainerState.Exception

```csharp
UnionContainer<int> container = Divide(5,0);
if(container.GetState() is UnionContainerState.Exception) //Match against the state of the container
{
    Console.WriteLine($"Encountered an exception {container.GetException()}");
}
```

```csharp
UnionContainer<int> container = Divide(5,0);
if(container.GetState() is UnionContainerState.Error) //Match against the state of the container
{
    Console.WriteLine("Container has the following errors");
    container.GetErrors().ForEachIfNotNull(error => Console.WriteLine($"\t {error}"));
}
```

```csharp
UnionContainer<int> container = Divide(0,0);
if(container.GetState() is UnionContainerState.Empty) //Match against the state of the container
{
    Console.WriteLine($"No result was provided, this can be used as a standin for a null");
}
```

```csharp
UnionContainer<int> container = Divide(4,2);
if(container.GetState() is UnionContainerState.Result) //Match against the state of the container
{
    Console.WriteLine($"Result stored in the container is {container.}");
}
```

UnionContainers also has extension methods for checking each state. 
```csharp
UnionContainer<int> container = Divide(5,0);

if (container.IsMissingResult())
{
    //Do stuff knowing the container has no result
}
if (container.IsEmpty())
{
    //Do stuff when container is an empty state
}
if (container.HasErrors())
{
    //Do stuff when container has errors
}
if (container.HasResult())
{
    //Do stuff when the container has a result
}
```

### `Match`
- Match has two versions, one that returns a result and one that does not.

```csharp
UnionContainer<string> resultNameContainer = GetNameIfNameInList("John Doe");

resultNameContainer.Match
(
    onResult: name => Console.WriteLine($"Employee found with name {name}"),
    onNoResult: () => Console.WriteLine("No employee found with that name"),
    onErrors: errors => errors!.ForEachIfNotNull(e => Console.WriteLine($"\t{e.GetName()}: {e.Message}")),
    onException: ex => Console.WriteLine(ex.Message)
);
```
In the above example, the Match method is used to print different messages depending on if the name is in the list, or if some sort of issue was encountered.

```csharp
public static void TestMethod10(UnionContainer<string,Guid> nameorId)
{
    UnionContainer<Programmer, Manager> container = Program.GetEmployeeOrManagerByNameOrId(nameorId);
    
    (string bonusReason,int bonus) = container.Match(
        programmer =>
        {
            int programmerBonus = 1000;
            if (programmer.Name is "John Doe")
            {
                return ("John Doe is being promoted to manager soon, up his bonus to match",(programmerBonus *2));
            }
            return ("Programmers get a standard bonus of ",programmerBonus);
        }, manager =>
        {
            if (manager.StartDate < (DateTime.Now - TimeSpan.FromDays(30)))
            {
                return ("Recently promoted managers receive a reduced bonus",2000);
            }
            return ("Managers in the role longer then 30 days, are entitled to a full managerial bonus",5000);
        },
        () => ("Employee is not a programmer or manager",500));
    
    Console.WriteLine($"The bonus is ${bonus} USD, because {bonusReason}");
}
```
In the above example, a container is used as a method argument to find the employee either by a name or an ID value. 

Then the container is matched and returns a bonus amount and a reason for the person depending on the employee type, or other desired checks.

If no specific employee is returned from the method call, the default match is invoked which returns the amount of 500.

### `If{State}Do` Methods
- These are extension methods that allow performing an action if the container is the specified state.
- One exists for each of the 3 non result states, result actions can only be performed in calls to match.

```csharp
var container = new UnionContainer<int>();

container.IfEmptyDo(() => "No result was returned or result was null");
container.IfExceptionDo((ex) => $"While doing xyz, the method failed and produced this exception {ex}");
//IfErrorDo...
```

The containers also contain the `ForState` methods, which perform an action if the container is in the specified state. 
```csharp
UnionContainer<int> container = Divide(1,0);

container.ForState(
    null,
    (UnionContainerState.Result, () =>Console.WriteLine("The container has a result")),
    (UnionContainerState.Error, () => Console.WriteLine("The container has an error")),
    (UnionContainerState.Exception, () => Console.WriteLine("The container has an exception")),
    (UnionContainerState.Empty, () => Console.WriteLine("The container has no result"))
    );
    
```

### `Error Related` Methods
UnionContainers has a few methods for adding and checking errors
```csharp
public static UnionContainer<Programmer> TestMethod6(string name)
{
    List<IError> errors = new();

    if (name.Length < 2)
    {
        errors.Add(ClientErrors.ValidationFailure("Name is too short"));
    }

    if (name.Length > 100)
    {
        errors.Add(ClientErrors.ValidationFailure("Name is too long"));
    }

    if (name.Contains(" ") is false)
    {
        errors.Add(ClientErrors.ValidationFailure("A first and last name is required"));
    }

    if (string.IsNullOrWhiteSpace(name))
    {
        errors.Add(ClientErrors.ValidationFailure("Name cannot be empty or whitespace only"));
    }

    if (errors.Count > 0)
    {
        Console.WriteLine($"Test Method 6 finished with {errors.Count} errors");
        //return errors;
        var returnContainer = new UnionContainer<Programmer>();
        foreach (IError error in errors)
        {
            returnContainer.AddError(error.GetMessage());
        }
        return returnContainer;
    }

    return new Programmer(name, Guid.NewGuid(),DateTime.UtcNow);
}
```
In the above example, if the passed in name value fails to meet various condtions errors are added to a list and then that list is returned inside the container.
It is also possible to add a list of errors with the `AddErrors` method instead of 1 at a time.

These can then be obtained with a call to the `GetErrors` method
```csharp
UnionContainer<Programmer> container = TestMethod6("H");
if(container.GetState() is UnionContainerState.Error)
{
    container.GetErrors().ForEachIfNotNull(error => Console.WriteLine(error));
}
```

### `ToContainer` Extension Methods
- UnionContainers also supplies two extension methods to convert existing objects into containers. 

- `ToContainer` => applies to all types except nullable value types, will wrap said item into a container returning an empty container if null
- `FromNullableToContainer` => wraps a object in the same way as ToContainer but also works on Nullable value types like `int?`, `char?`, etc.

```csharp
char? letterGrade = quarterAverage switch
{
    <= 100 and >= 90 => 'A',
    <= 89 and >= 80 => 'B',
    <= 79 and >= 70 => 'C',
    <= 70 and >= 60 => 'D',
    <= 60 and >= 50 => 'F',
    _ => null
};

UnionContainer<char> charContainer = letterGrade.FromNullableToContainer();
```

```csharp
string hello = "hello";
UnionContainer<string> stringContainer = hello.ToContainer();
```

## Automatic Exception Conversion & Error Handling Registration
The UnionContainers library also contains a few methods for helping convert Exceptions to Errors, and for automatically handling specific Error types.
When setting up your application the `AddUnionContainerConfiguration` method can be called to setup DI for the UnionContainer library.

```csharp
builder.Services.AddUnionContainerConfiguration(config =>
{
    config.AddDefaultErrorConverters();
    config.TryRegisterErrorHandler<ClientErrors.InvalidOperationError>(new ErrorHandler<ClientErrors.InvalidOperationError>((error) => Console.WriteLine($"Invalid Operation Error: {error.Message}")));
});
```

The `AddDefaultErrorConverters` method will add the default error converters for the UnionContainer library, which will convert the following exceptions to the following error types:
- ArgumentNullException => ClientErrors.ArgumentNullError
- ArgumentException => ClientErrors.ArgumentError
- InvalidOperationException => ClientErrors.InvalidOperationError
- NullReferenceException => ClientErrors.ValidationFailureError
- HttpRequestException => NetworkErrors.NetworkingError


The `TryRegisterErrorHandler` method will register a custom error handler for a specific error type. This is useful when you know you want to always perform the same action when a specific type of error occurs.

This could be used to write to a log file, send an email, or perform some other action when a specific error occurs.
If a error handler is not registered or if a OnError method is provided when matching a container, then the error handler is not used.

## Performance

The `UnionContainers` library is designed to be as performant as possible, and in some cases is faster than choosing to use native types.
The following benchmarks are the result of running the `UnionContainers.Benchmarks` project. In it a random 100-people objects are created and matched against a random name to return either the person or a null object.
In this case UnionContainers was not only faster than using native nullable types, but also faster them similar libraries like OneOf and,LanguageExt.

```csharp
| Method                    | Mean     | Allocated |
|-------------------------- |---------:|----------:|
| TryGetConsultant          | 508.7 ns |     168 B |
| TryGetConsultantOneOf     | 503.5 ns |     168 B |
| TryGetConsultantLangExt   | 501.0 ns |     168 B |
| TryGetConsultantUnionCont | 455.7 ns |     168 B |
```



## Contributing
Feel free to contribute to this project by submitting issues, feature requests, or pull requests.

## License
UnionContainers is licensed under the MIT License (i.e., fully open source). See the LICENSE file for more information.
