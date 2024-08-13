# Union Container
`UnionContainers` is a library that provides a discriminated union like type in C#.
It is not a pure copy of a discriminated union, but it is a close approximation with some extras mixed in to make working with this type as painless as possible in existing or new C# projects.

The `UnionContainers` library uses Roslyn source generators & diagnostic analyzers to assist in the creation and usage of `UnionContainers`.
This helps to ensure compile time checks are performed to minimize runtime errors. It also provides the ability to have type-safe, strongly typed compile-time code without the overhead of reflection or dynamic types.

## Features & Benefits
- **Type Safety**: The `UnionContainers` library provides type safety by using Roslyn diagnostic analyzers to prevent the assignment of invalid types to `UnionContainers` & from `UnionContainers`.
- **Compile Time Checks**: The `UnionContainers` library provides several compile time checks to ensure that the `UnionContainers` are used correctly.
- **Little Reflection**: The `UnionContainers` library only uses reflection when required (ex. match dynamic delegates to the container type), it uses generics to enforce strongly typed containers.
- **Avoid Null Reference Exceptions**: The `UnionContainers` library helps to avoid null reference exceptions by providing a way to handle empty containers.
- **Set custom error messages**: `UnionContainers` can hold error messages for non-exceptional failures such as not finding an item in a list or failing a validation check.
- **Propagate Exceptions**: `UnionContainers` can hold exceptions that are thrown during the execution of a function, this allows the caller to receive the full exception for inspection to determine the best path forward without need a bunch of try catch blocks.
- **Wrap non-user defined types**: The `UnionContainers` library can execute a supplied method call using the `MethodToContainer` method and wrap the result in an `UnionContainer`. This includes wrapping any exceptions so that they can be handled by the caller as desired.
- **Limit allowed types**: The `AllowedTypes` & `DeniedTypes` attributes can be used to limit the types allowed in a property, field, method parameter, method return type, class generic type parameter, or method generic type parameter.
- **Avoid needless method overrides**: Since Union Containers can hold multiple types, they can be used to avoid the need for multiple overrides of a method that take different types of arguments to perform the same action. ex. Finding a user by name or id.

## Installation
Interested in trying the library out? You can install it via NuGet Package Manager Console by running the following command:
```bash
Install-Package UnionContainers
```
Next, add a using statement to the top of your file to use the library
```csharp
using UnionContainer.Core.Common;
using UnionContainer.Core.Helpers;
using UnionContainer.Core.UnionContainers;
using UnionContainers.Shared.Common;
```

### Demo App 
Test the Demo App and see various examples of the UnionContainers, Functional Methods, and Allowed Types Aspects.
1. Git clone
2. Build
3. Run DemoApp
4. Review demo app source code examples


## What is a UnionContainer? 
A Union Container is a type that can be in one of four states: 
    
- **Empty** - The container is empty and has no value.
- **Valid Value** - The container has a valid value of one of the possible types.
- **Error** - The container has a custom error value set.
- **Exception** - The container has an exception value set.

All containers start in an empty state and only transition out of that state when a value of a valid type is set or optionally when an exception/error is present.

an example of a simple `UnionContainer` is shown below:

```csharp
public static UnionContainer<int> DivideByZeroTestContainer()
{
    UnionContainer<int> container = new UnionContainer<int>();
    container =  MethodToContainer<int>(() => RandomExampleMethods.Divide(10,0));
    return container;
}
```
The divide method mentioned above looks like this:
```csharp
public static int Divide(int a, int b)
{
    return a / b;
}
```
The `MethodToContainer` method is a helper method that will execute the supplied function and wrap the result in an `UnionContainer`. 
If the function throws an exception, the exception will be caught and placed in the Exception value of the`UnionContainer`.

the container can then be inspected in a number of ways to determine the state of the container and act accordingly.
The most straightforward way is just to call the different state checking extension methods.
```csharp
if (container.IsEmpty)
{
    Console.WriteLine("Container is empty");
}
else if (container.HasValue)
{
    Console.WriteLine($"Container has a value: {container.Value}");
}
else if (container.HasError)
{
    Console.WriteLine($"Container has an error: {container.Error}");
}
else if (container.HasException)
{
    Console.WriteLine($"Container has an exception: {container.Exception}");
}
```

In the previous example the output would be
```text
container is Empty
Container has an exception: System.DivideByZeroException: Attempted to divide by zero.
```

## Why UnionContainers? 
The library provides the features mentioned above and showcased in the rest of the ReadME. 
I created this library with the goal of having a result type that could contain results, errors, and exceptions, allow for returning the result item, ingest additional methods, and return results based on the results value. 

It can be used in any situation where the return state is not guaranteed for example 
1. Database Query
2. Web APIs
3. Non-user written methods that throw an exception (ex. HttpClient)
4. Getting items from a list
5. As an argument to avoid method overloads for slight variations in argument types (ex. ID as string, int, or GUID)
6. Return types from user-written methods where more context and variation in return types is desired

the following is an example of querying a LiteDB database for a type of item that may not exist in the database 
```csharp
public async Task<UnionContainer<IEnumerable<T>>> GetAllFromDatabaseAsync<T>()
{
    try
    {
        var container = new UnionContainer<IEnumerable<T>>();
        var foundItems = await _database.GetCollection<T>().FindAllAsync();
        if(foundItems != null && foundItems.Any())
        {
            return container.SetValue(foundItems);
        }
        return container.AddError($"No items of type {typeof(T)} found in database.");
    }
    catch (Exception e)
    {
        return UnionContainerFactory.CreateWithException<IEnumerable<T>>(e);
    }
}
```
This then allows the caller to inspect the states they care about and inspect any errors or exceptions in the database method added to the container.
```csharp
public async void Example()
{
    UnionContainer<IEnumerable<ExampleType>> databaseResultContainer = await GetAllFromDatabaseAsync<ExampleType>();
    databaseResultContainer.IfErrorDo<string>(x => Console.WriteLine(x));
    databaseResultContainer.IfExceptionDo(x => Console.WriteLine(x));
    databaseResultContainer.IfEmptyDo(() => Console.WriteLine("No value set in container return type"));
    databaseResultContainer.TryHandleResult(() =>
    {
        foreach (ExampleType exampleType in databaseResultContainer.TryGetValue())
        {
            Console.WriteLine(exampleType);
        }
    });
}
```



## Basic Usage

### Null Handling
Containers will also gracefully handle null values and be empty if a null value is assigned to them.
```csharp
UnionContainer<int> container = null;
if (container.IsEmpty)
{
    Console.WriteLine("Container is empty");
}
if (container.HasValue)
{
    Console.WriteLine($"Container has a value: {container.Value}");
}
```
the above will output `container is empty` as the container is null and, therefore, never left the empty state.

### Empty State
The `Empty` state is a state that is set to a struct that is meant to contain no values and acts as a replacement for null/void values.
```csharp
[Serializable]
[StructLayout(LayoutKind.Sequential, Size = 1)]
public record struct Empty
{
    public static Empty Nothing { get; } = new Empty();
    
    public static Empty Return() => Nothing;
}
```
The `Empty` static class contains instances of these types that can be used to set the state of a container to empty.
```csharp
public static class Empty
{
    /// <summary>
    /// A Empty instance <br/>
    /// Used as a replacement for Default / no data values <br/>
    /// </summary>
    public static Empty Default = new Empty();
    
    /// <summary>
    /// A NullEmptyType instance <br/>
    /// Used to represent null values while avoiding being truly null <br/>
    /// </summary>
    public static NullEmptyType Null => new NullEmptyType();
}
```
This means anything in the Empty state refers to one of these instances and therefor is never in a true null state avoiding pesky null reference exceptions.
This is similar to the Unit type in other languages. The empty type can also be used in place of a void return type / null to represent a lack of a value.


### UnionContainer Types
The other primary benefit of the Union Containers is that they can be created with allowed values ranging from T1 - T16.
This means you can create a container that may contain a value for one of up to 16 different possible types.

```csharp
var container = MethodToContainer(() =>
{
    HttpClient client = new HttpClient();
    var result = client.GetAsync("http://127.0.0.1:8080/").Result;
    if (result.StatusCode != HttpStatusCode.OK)
    {
        return result.StatusCode;
    }
    return result.Content.ReadAsStringAsync().Result;
});
```

In the above example, the container will be a `UnionContainer<string,HttpStatusCode>` and can contain either a `string` or a `HttpStatusCode` value.
It can also contain an error or exception if one is thrown during the execution of the function.

The `UnionContainers` library also provides a way to create a container with a custom error message.
This can be useful when you want to provide a custom error message to the caller.
```csharp
UnionContainer<int?> container = myNumberFunction();

if (container.TryGetValue() is 42)
{
    container.SetErrorState(Status.Rejected,"Container cannot be 42",DateTime.Now);
}

if (container.HasError())
{
    Console.WriteLine($"Container has an error: {container.GetErrorValues()}");
}
```
In the above example we wrap a nullable int in an `UnionContainer` and then set an error state with a custom error message if the value is 42.
The `TryGetValue` call only succeeds if an int was set in the container, this can be used together with the `is` operator to check the value of the container in a safe way.
the Error type is a list of objects and new items are cast to the type of the first error and added to the list, this allows for a single error state to hold multiple error messages.


### Creating Union Containers
Union Containers can be created in a few different ways some are implicit and some are explicit.
#### implicit examples 
```csharp
public class ImplicitContainerCreation
{
    public static void ImplicitContainerCreationExample()
    {
        // implicit conversion
        Console.WriteLine("Implicit container creation example from method result");
        UnionContainer<Employee, Manager> container_two = TryGetEmployeeByName(targetNameTwo); // implicit because it returns an employee type
    }
    
    public static void ImplicitContainerCreationExampleTwo()
    {
        // implicit conversion
        Console.WriteLine("Implicit container creation example from object conversion");
        UnionContainer<string, int, HttpStatusCode> container = HttpStatusCode.OK;
    }
}
```

#### Explicit examples
```csharp
public class ExplicitContainerCreation
{
    public static void ToUnionContainerExample()
    {
        Console.WriteLine("ToUnionContainer Example");
        UnionContainer<Employee> container = TryGetEmployeeByName(targetNameTwo).ToUnionContainer();
    }

    public static void ContainerFromMethodExample()
    {
        // direct assignment but uses the implicit conversion operator for the argument
        Console.WriteLine("ContainerFromMethod Example");
        UnionContainer<Employee, Manager> container = GetEmployeeOrManagerByNameOrId(targetManagerName); 
    }


    /// <summary>
    /// Example of using the MethodToContainer method to wrap a method that returns a value
    /// In this example it is a method that the user does not control and would have the ability to ensure it would properly handle exceptions, produce errors, etc.
    /// The user would also be incapable of modifying the source of the method to return an UnionContainer but the MethodToContainer method allows the user to safely execute the method and handle the result
    /// </summary>
    public static void NonUserMethodToContainer()
    {
        HttpClient client = new HttpClient();
        var container = MethodToContainer<UnionContainer<HttpResponseMessage>,HttpResponseMessage>(() => client.GetAsync("http://127.0.0.1:8080/").Result);
       
        container
            .IfEmptyDo(() => Console.WriteLine("Container is empty"))
            .TryHandleResult((HttpResponseMessage response) =>
            {
                Console.WriteLine("Container has a response: " + response);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    container.AddErrorValue($"Failed with error message {response.StatusCode}");
                }
            })
            .IfErrorDo<string>((errors) => Console.WriteLine($"Container has 1 or more errors: \n\t {errors.ToCommaSeparatedString()}"))
            .IfExceptionDo((exception) =>
            {
                Console.WriteLine("Container has an exception: " + exception.Message);
            });
    }
    
   
    /// <summary>
    /// Example where an existing user function is passed into the MethodToContainer method this allows the safe execution of the method and the handling of the result without needing to modify existing source
    /// </summary>
    public static void UserFunctionWrapperExample()
    {
        var container = MethodToContainer<UnionContainer<HttpResponseMessage>,HttpResponseMessage>(() => TryConnectLocalhost("localhost", "http", 5005));

        container
            .IfEmptyDo(() => Console.WriteLine("Container is empty"))
            .TryHandleResult((HttpResponseMessage result) =>
            {
                Console.WriteLine("processing result");
                HttpStatusCode returnValue = GetConnectionResponseCode(result);
                Console.WriteLine("Finished processing result return value: " + returnValue);
                if (returnValue != HttpStatusCode.OK)
                {
                    container.AddErrorValue(returnValue);
                }
            })
            .IfErrorDo<HttpStatusCode>((errors) => Console.WriteLine($"Container has an error: \n\t {errors.ToCommaSeparatedString()}"))
            .IfExceptionDo((exception) => Console.WriteLine("Container has an exception: " + exception.Message));
    }
    
    public static HttpStatusCode GetConnectionResponseCode(HttpResponseMessage response)
    {
        Console.WriteLine("response: " + response);
        return response.StatusCode;
    }
}
```

### Using Union Containers
Union Containers are used in a variety of ways, including as previously mentioned as a wrapper for a function call, they can also be used directly as method argument values or as a return type.

```csharp
public static UnionContainer<Employee> TryGetEmployeeByNameOrId(UnionContainer<string,int,Guid> nameOrId)
{
    var container = new UnionContainer<Employee>();
    //create 5 new employees
    var employees = new List<Employee>
    {
        new Employee("John Doe",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
        new Employee("Jane Doe",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
        new Employee("Bob Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
        new Employee("Sally Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
        new Employee("Joe Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow)
    };
    if(nameOrId.IsEmpty())
    {
        Console.WriteLine("Trying to get name or id:  No name or id provided");
        container.AddErrorValue("No name or id provided");
    }
    Employee? employee = null;
    nameOrId.TryHandleResult((string name) =>
    {
        Console.WriteLine($"Trying to get employee by name {name}: searching names");
        if(name == "Bob Stevens")
        {
            container.AddErrorValue("Bob Stevens is on vacation");
        }
        employee = employees.FirstOrDefault(e => e.Name == name);
        employee.IfNullDo(() => container.AddErrorValue("No employee found with that name"));
    })
    .TryHandleResult((int idNumber) =>
    {
        Console.WriteLine($"Trying to get employee by id {idNumber}: searching id values");
        employee = employees.FirstOrDefault(e => e.ID == new Guid(idNumber.ToString()));
        employee.IfNotNullDo((e) => container.SetValueState(e));
        employee.IfNullDo(() => container.AddErrorValue("No employee found with that id"));
    })
    .TryHandleResult((Guid id) =>
    {
        Console.WriteLine($"Trying to get employee by id {id}: searching id values");
        employee = employees.FirstOrDefault(e => e.ID == id);
        employee.IfNotNullDo((e) => container.SetValueState(e));
        employee.IfNullDo(() => container.AddErrorValue("No employee found with that id"));
    });
    return container;
}
```
In the above example we can see a Union Container of `UnionContainer<string,int,Guid>` is passed into this method as an argument this means we know the value will not be null & will be one of the three types.
We can then use a match expression to determine which type the value is and return the correct employee from the list of employees. If an employee is not found the container is kept empty and an error state is set.
This also avoids the need for returning null or throwing exceptions for non-exceptional cases.
Using UnionContainers in this manner to present a few similar options for an argument allows for the creation of 1 single method with can then handle multiple types of input and return the correct value vs needing to create an override for name as a string, id as an int, id as a guid etc.

when invoking code like what is above the caller does not have to pass in a Union Container but can pass in any of the allowed argument values
```csharp
UnionContainer<Employee> container = new container = TryGetEmployeeByNameOrId("Jane Doe");
if(container.HasResult())
{
    Container.TryHandleResult((Employee resultItem) =>
    {
       Console.WriteLine("Container has a result");
       Console.WriteLine($"Employee found: {resultItem.Name}");
    });
}
```

We also see a Union Container is returned from this method, this is a common pattern when using Union Containers as it allows the caller to handle the different outcome states in a safe way.
For example this function might not find an employee with the provided name or id and need to return an error message about what went wrong, it might also want to perform some validation logic and return an error state if the validation fails.
Lastly it might throw an exception if the code is not functioning as expected. The container produced will contain the whole exception and can be inspected by the caller to determine what went wrong and how to best handle it.

### Updating the state of the container 
The state of the container can be set with the `SetEmptyState`, `SetErrorState`, `SetExceptionState`, and `SetValueState` methods.
Each of these methods will set the state of the container to the provided state and return the container to allow for method chaining.

```csharp
//create a container of type employee,Manager
UnionContainer<Employee,Manager> container = new();

//valid, result update method
container.SetValueState(employee); 
//valid derived type result update method
container.SetValueState(newHire); 
//valid result update method
container.SetValueState(manager1);
//valid derived type result update method
container.SetValueState(managerInTraining);
// InValid type, errors with UNCT 001
container.SetValueState(hrPerson); 
//InValid derived type, errors correctly with UNCT 001 now 
container.SetValueState(hrPersonInTraining); 

// Valid direct assignment 
container = employee; 
// Valid direct assignment of derived type
container = newHire;
// Valid direct assignment, when null the container stays at Empty state
manager1 = null;
container = manager1; 
// Valid direct assignment of derived type
container = managerInTraining; 
// InValid type, correctly errors
container = hrPerson; 

// valid exception state update method
container.SetExceptionState(new Exception("an exception occurred");
    
// valid error state update method , arguments can be as many as desired and any type as desired
container.SetErrorState("An error occurred", "A second error message", 5); 

// valid empty state update method -> containers start in this state but if for some reason it needed to be reset this is how
container.SetEmptyState();
```


### Getting the value out of a container 
Getting the value out of a container is done in a few different ways depending on the state of the container.
It is not required to supply logic for all states of a container, but it is recommended to do so to ensure that all possible states are handled correctly / accounted for.

#### Issue State matching 
- an **issue state** is categorized as anything but a valid value state ex.(Empty, Error, Exception)

```csharp
namespace DemoApp.ContainerResultMatchExamples;

public class IssuesMatch
{
    /// <summary>
    /// Method that demonstrates how to check if a container has issues
    /// A container has issues if it is empty, has an error, or has an exception
    /// An optional HandleIssues method can be used to handle the issues taking Actions as arguments
    /// </summary>
    public static void HasIssuesExample()
    {
        UnionContainer<Employee> container = TryGetEmployeeByName(targetNameTwo).ToUnionContainer();
        container.SetExceptionValue(new Exception("An exception occurred"));
        if (!container.HasIssues())
        {
            return;
        }
        Console.WriteLine("Container one has issues");
        container
            .IfEmptyDo(() => Console.WriteLine("Container one is empty"))
            .HandleIssues<string>(
                isError: (errors) => Console.WriteLine($"Container one has an error \n error values: {errors.ToCommaSeparatedString()}"),
                isException: exception => Console.WriteLine("Container one has an exception: " + exception.Message));
    }

    /// <summary>
    /// Method that demonstrates how to check if a container has issues
    /// A container has issues if it is empty, has an error, or has an exception
    /// Instead of the HandleIssues method, the IsEmpty, HasError, and HasException methods can be used to check the state of the container
    /// Not all states need to be accounted for in the code if they are not relevant
    /// </summary>
    public static void HasIssuesSingleMatchExample()
    {
        UnionContainer<Employee> container = TryGetEmployeeByName(targetNameTwo).ToUnionContainer();
        container.AddErrorValue("An error occurred").AddErrorValue("A second error occurred"); 
        if (container.HasIssues())
        {
            Console.WriteLine("Container two has issues");
            if (container.HasError())
            {
                Console.WriteLine($"Container two has an error \n error values: {container.GetErrorValues<string>().ToCommaSeparatedString()}");
            }
            else if (container.HasException())
            {
                Console.WriteLine("Container two has an exception: " + container.GetException());
            }
            else if (container.IsEmpty())
            {
                Console.WriteLine("Container two is empty");
            }
        }
    }


    /// <summary>
    /// Method showing the use of the fluent method chaining to check container status and execute the provided actions
    /// </summary>
    public static void IfIssuesMatch()
    {
        UnionContainer<Employee> container = TryGetEmployeeByName("Not real").ToUnionContainer();
        container
            .IfEmptyDo(() => Console.WriteLine("Container is empty"))
            .IfErrorDo<string>((errors) => Console.WriteLine($"Container has an error \n error values: {errors.ToCommaSeparatedString()}"))
            .IfExceptionDo((exception) => Console.WriteLine("Container has an exception: " + exception.Message));
    }
}
```

#### Valid Value State matching
- a **valid value state** is categorized as a state that has a value of one of the possible types.

```csharp
namespace DemoApp.ContainerResultMatchExamples;

public class ResultMatch
{
    /// <summary>
    /// Containers can be matched using the deconstruction syntax, when using this syntax it is recommended to use the is keyword to check the container result to ensure null safety type
    /// It is also recommended to discard the unused value with the _ placeholder
    /// </summary>
    public static void DeconstructionMatch()
    {
        UnionContainer<Employee,Manager> container = GetEmployeeOrManagerByNameOrId("Jane Doe");
        if (container is (Employee employee, _))
        {
            Console.WriteLine("Found employee with ID: " + targetGuid);
            Console.WriteLine($"info: {employee.Name} is a {employee.JobTitle} and makes {employee.Salary} as of {employee.StartDate}");
        }
        else if (container is (_, Manager manager))
        {
            Console.WriteLine("Found manager with ID: " + targetGuid);
            Console.WriteLine($"info: {manager.Name} is a {manager.JobTitle} and makes {manager.Salary} as of {manager.StartDate}");
        }
    }

    /// <summary>
    /// The TryGetValue method can be used to get the value of the container
    /// </summary>
    public static void TryGetValueExamples()
    {
        //getting the value of the container
        UnionContainer<Employee, ManagerInTraining> containerTwo = employee;
        Employee? containerValue;
        ManagerInTraining? containerValueTwo;
        
        containerTwo.IfEmptyDo(() => Console.WriteLine("Container is empty"))
            .TryHandleResult((Employee employee) => Console.WriteLine($"Container value is an employee \n\t{employee}"))
            .TryHandleResult((ManagerInTraining manager) => Console.WriteLine($"Container value is a manager \n\t {manager}"));
    }
    
    
    /// <summary>
    /// Example method where the container is checked to be in a state of result before trying to get the value
    /// </summary>
    public static void IsResultExamples()
    {
        UnionContainer<Employee> container = GetEmployeeByNameOrId("Jane Doe");
        if(container.HasResult())
        {
            Employee employee1 = container.TryGetValue().ThrowIfNull();
            Console.WriteLine("container_one is an employee");
            Console.WriteLine($"info: {employee1.Name} is a {employee1.JobTitle} and makes {employee1.Salary} as of {employee1.StartDate}");
        }
    }


    public static void HandleResultExamples()
    {
        var container = MethodToContainer<string,HttpStatusCode>(() =>
        {
            HttpClient client = new HttpClient();
            var result = client.GetAsync("http://127.0.0.1:8080/").Result;
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return result.StatusCode;
            }
            return result.Content.ReadAsStringAsync().Result;
        });
        
        //Produces compile error UNCT007 because the container only supports 2 generics but was given 3 arguments
        /*container.TryHandleResult(
            (string responseBody) => Console.WriteLine("Got a response of: " + responseBody),
            (HttpStatusCode statuscode) => Console.WriteLine("Got a response status code of : " + statuscode),
            () => Console.WriteLine("Failed to get a response"));*/

        container.TryHandleResult(
            (string responseBody) => Console.WriteLine("Got a response of: " + responseBody),
            (HttpStatusCode statuscode) => Console.WriteLine("Got a response status code of : " + statuscode));
        
        UnionContainer<Employee, ManagerInTraining> containerTwo = employee;
        containerTwo.IfEmptyDo(() => Console.WriteLine("Container is empty"))
            .TryHandleResult((Employee employee) => Console.WriteLine($"Container value is an employee \n\t{employee}"))
            .TryHandleResult((ManagerInTraining manager) => Console.WriteLine($"Container value is a manager \n\t {manager}"));
    }
}
```

#### TryGetValue
While `TryGetValue` is the easiest way to get a value from a container it can result in a null return value.
```csharp
//Single generic type container
UnionContainer<string> container = "Hello World";
string? containerValue = container.TryGetValue();
Console.WriteLine($"Container value: {containerValue}");
```
 
It is recommended to always perform a type check when getting the value out such as `if (container.TryGetValue() is Employee _employee)` to ensure that the value is of the correct type.

As a note of caution when using `TryGetValue` on a multi container type it is better to use the HandleResult call, this prevents the need for the type checking and avoids runtime errors. 
```csharp
//Ok
UnionContainer<string> container = "Hello World";
string? containerValue = container.TryGetValue();
if(containerValue != null)
{
    Console.WriteLine($"Container value: {containerValue}");
}

//Better
UnionContainer<string> container = "Hello World";
container.TryHandleResult((string value) => Console.WriteLine($"Container value: {value}"));
```

However, if desired the `TryGetValue` method can be given an optional value/method to supply a backup return value in the case where the container is empty, in a null state, or contains an error.
```csharp
UnionContainer<string> container = null;
string containerValue = container.TryGetValue(fallbackValue: "No value found");
Console.WriteLine($"Container value: {containerValue}");
```
```csharp
UnionContainer<string> container = null;
string containerValue = container.TryGetValue(fallbackValueMethod: () => 
{
    Console.WriteLine("No value found, please enter a value");
    return Console.ReadLine();
});
Console.WriteLine($"Container value: {containerValue}");
```



#### HandleResult
Handle results lets you stay in the context of a container while providing a way to execute different actions based on the type of the value in the container.
This prevents any miscasts or null reference exceptions that could occur when trying to get the value out of the container and then perform actions on it.
```csharp
UnionContainer<Employee,Manager> container = GetEmployeeOrManagerByNameOrId("Jane Doe");
container.TryHandleResult(
    //executes if the value is an employee
    employee => Console.WriteLine($"Found employee \n info: {employee.Name} is a {employee.JobTitle} and makes {employee.Salary} as of {employee.StartDate}"),
    //executes if the value is a manager
    manager => Console.WriteLine($"Found manager \n info: {manager.Name} is a {manager.JobTitle} and makes {manager.Salary} as of {manager.StartDate}"));
```
`TryHandleResult` also contains options for a catch handler method to be passed in `Func<Exception,T>? catchHandler` which can be used to handle exceptions that occur during the execution of the passed in methods.
```csharp
container.TryHandleResult((Employee employee) =>
        {
            Console.WriteLine($"Found employee returning name: {employee.Name}");
            return employee.Name;
        }, 
        catchHandler: (exception) =>
        {
            Console.WriteLine($"Error occurred: {exception.Message}");
            return "exception";
        },  fallbackValue: "none");
```
There is also an Action override if you do not need to return a value from the method passed in.

##### GetMatchedItem
The `GetMatchedItem` method can be used to get the value of the container in a safe way. It also has a matching method `GetMatchedItemAs<T>` that can be used to cast the matched item to a specific type.
The matched item is NOT the result of the container, instead when a method is passed in to handle the result of the container the matched item is the value that was matched or returned by the method.
The value returned from the executed method is then cleared and a new method can be executed to handle the result in a different way.
```csharp
public static void ContainerSingleHandleValueExtract()
    {
        UnionContainer<Employee,Manager> container = GetEmployeeOrManagerByNameOrId("Jane Doe");
        
        Console.WriteLine("Example of a container matching the correct type & returning a result for the testValue variable");
        string containerExtractedName = container.TryHandleResult((Employee employee) =>
        {
            Console.WriteLine($"Found employee returning name: {employee.Name}");
            return employee.Name;
        }, fallbackValue:"none").GetMatchedItemAs<string>()!;
        
        string containerExtractedName3 = container.TryHandleResult((Employee employee) =>
        {
            Console.WriteLine($"Found employee returning name: {employee.Name}");
            return employee.Name;
        }, fallbackValue:"none").GetMatchedItemAs<string>()!;
        
        Console.WriteLine("Example of a container matching a different type, but having a fallback value set for the testValue2 variable");
        int containerExtractSalary = container.TryHandleResult((Manager manager) =>
        {
            Console.WriteLine($"Found employee returning salary: {manager.Salary}");
            return manager.Salary;
        }, fallbackValue: 200000).GetMatchedItemAs<int>();
        
        Console.WriteLine($"Extracted a string from the container: {containerExtractedName}");
        Console.WriteLine($"Extracted an int from the container: {containerExtractSalary}");
    }
    
    
    public static void ContainerMultiHandleValueExtract()
    {
        UnionContainer<Employee,Manager> container = GetEmployeeOrManagerByNameOrId("Jane Doe");
        
        //Goal here is that we want to extract the name of the employee or manager & set that as the value of the container
        //Otherwise we just have an Empty container which is fine to ignore
        UnionContainer<string> nameContainer = container
        .TryHandleResult((Employee employee) => employee.Name)
        .TryHandleResult((Manager manager) => manager.Name)
        .GetMatchedItemAs<string>()!;
        
        
        UnionContainer<string> nameContainer2 = (string)container
        .TryHandleResult((Employee employee) => employee.Name)
        .TryHandleResult((Manager manager) => manager.Name)
        .GetMatchedItem()!;
       
        
        nameContainer
        .IfEmptyDo(() => Console.WriteLine("Name container is empty"))
        .TryHandleResult(name => Console.WriteLine($"Name Container Value: {name}"));
        
        nameContainer2
        .IfEmptyDo(() => Console.WriteLine("Name container 2 is empty"))
        .TryHandleResult(name => Console.WriteLine($"Name Container 2 Value: {name}"));
    }

    public static void ContainerMultiHandleUniqueTypeExtraction()
    {
        UnionContainer<Employee,Manager> container = GetEmployeeOrManagerByNameOrId("Jane Stevens");
        container
        .TryHandleResult((Employee employee) => employee.Name)
        .TryHandleResult((Manager manager) => manager.Salary);

        UnionContainer<string,int> nameOrSalaryContainer = new();
        nameOrSalaryContainer
        .SetValue(container.GetMatchedItemAs<string>())
        .SetValue(container.GetMatchedItemAs<int>());
        
        nameOrSalaryContainer.TryHandleResult((int salary) => Console.WriteLine($"Manager Salary: {salary}"));
        nameOrSalaryContainer.TryHandleResult((string name) => Console.WriteLine($"Employee Name: {name}"));
        ExampleMethod();
    }
```

looking at the above example we can see where a method was passed in to handle the result of the container and return the name of the employee stored in the container.
```csharp
string containerExtractedName = container.TryHandleResult((Employee employee) =>
{
    Console.WriteLine($"Found employee returning name: {employee.Name}");
    return employee.Name;
}, fallbackValue:"none").GetMatchedItemAs<string>()!;
```
the `GetMatchedItemAs<string>()` will only return a value if the method passed in is executed and returns a value, if the container is empty or in an error or exception state the `GetMatchedItemAs<string>()` will return null.
This can be accounted for by using the `fallbackValue` parameter to allow a default to be returned if the passed in method does not execute or errors out.


#### TryGetValue vs HandleResult with GetMatchedItem
Both of these methods will allow you to extract the result value from the container, but they are used in different ways.
TryGetValue is used when you want to get the value out of the container and then perform actions on it.
HandleResult with GetMatchedItem is used when you want to perform actions on the value in the container and then get the value out.
```csharp
public static void ExampleMethod()
{
    //Create a container
    UnionContainer<string, int> container = new UnionContainer<string, int>().SetValue(5);

    var number1 = container.TryGetValue<int>();
    Console.WriteLine($"The value is {number1}"); // prints 5
    

    container.TryHandleResult(
        (int value) =>
        {
            Console.WriteLine($"The value is an int that when added to 5 equals {value + 5}");
            return value + 5;
        });
    int number2 = container.GetMatchedItemAs<int>();
    Console.WriteLine($"The value is {number2}"); // prints 10
    
    container.TryHandleResult(
        (int value) =>
        {
            Console.WriteLine($"The value is an int that when added to 10 equals {value + 10}");
            return value + 10;
        });
    int number3 = container.GetMatchedItemAs<int>();
    Console.WriteLine($"The value is {number3}"); // prints 15
}
```
Here we see that when TryGetValue is used the int value is extracted from the container and is equal to 5.
When HandleResult with GetMatchedItem is used the container first ensures the value is an int, and then runs the supplied method adding 5 to it, 
the int value the method returns is then extracted with the GetMatchedItemAs method and is equal to 10.
Again since the value of the executed method is cleared after its extracted a new TryHandleResult method can be executed to handle the value in a different way, this time printing 15.


#### All State matching
- It is also possible to chain function calls together to set code execution paths for all the possible states of the container.

```csharp
namespace DemoApp.ContainerResultMatchExamples;

public class AllMatch
{
    /// <summary>
    /// Fluent match example for UnionContainer
    /// showcases responses for handling the various error states and results
    /// Uses the HandleResult method to handle the specific container result types
    /// </summary>
    public static void FluentMatchExample()
    {
        UnionContainer<Employee,Manager> container = GetEmployeeOrManagerByNameOrId("Jane Doe");
        
        container
            .IfEmptyDo(() => Console.WriteLine("Container three is empty"))
            .IfErrorDo<string>((errors) => Console.WriteLine($"Container three has an error \n error values: {errors.ToCommaSeparatedString()}"))
            .IfExceptionDo((exception) => Console.WriteLine("Container three has an exception: " + exception.Message))
            .ContinueWith(container)
            .TryHandleResult((Employee employee) =>
            {
                Console.WriteLine($"Found employee \n info: {employee.Name} is a {employee.JobTitle} and makes {employee.Salary} as of {employee.StartDate}"); 
            })
            .ContinueWith(container)
            .TryHandleResult((Manager manager) =>
            {
                Console.WriteLine($"Found manager \n info: {manager.Name} is a {manager.JobTitle} and makes {manager.Salary} as of {manager.StartDate}");
                return manager.Name;
            });
    }
}
```

## Compile time checks
Currently, the `UnionContainers` library provides a few compile time checks to ensure that the `UnionContainers` are used correctly.
- **UNCT001: Invalid argument type for UnionContainer creation or value setting** - This diagnostic is raised when an `UnionContainer` is created with a type that is not a valid type.
- **UNCT002:** - Not implemented
- **UNCT003:** - Not implemented
- **UNCT004: Invalid Container Conversion** - The target container type must contain all the generic types of the source container type.
- **UNCT005: Incompatible type assignment from TryGetValue** - Warns about potential type mismatches when assigning the result of TryGetValue to a variable.
- **UNCT006: Invalid type usage** - Ensures that the used type is one of the allowed types specified by the AllowedTypesAttribute.
- **UNCT007: Invalid HandleResult usage - Warning** - Detects when a chain of TryHandleResult calls might not handle all types from the original UnionContainer.
- **UNCT008: Invalid type usage** - Ensures that the used type is not one of the denied types specified by the DeniedTypesAttribute.
- **UNCT009: Invalid type usage** - Detects when TryHandleResult is invoked with incorrect types.
- **UNCT010** - Detects when a return type is not allowed Return types for MethodToContainer must be one of the containers specified types

**Check the demo project for examples of how to use the `UnionContainers` library.**


## Allowed Types & Denied Types Attribute
The `UnionContainers` library provides two attributes that can be used to specify the allowed and denied types in a few key places.
The goal here is to allow the developer the ability to limit a value to a specific set of types or to deny a specific set of types without forcing the use of Union Containers. 
Unlike a Union Container which uses strongly typed generics to ensure the correct type is used, these attributes are used to ensure that the correct types are used in the correct places but casting would still have to occur during runtime 

Both of these attributes can take between 1 and 16 types as generic type arguments, this is to allow for the most flexibility in the use of these attributes.
they can be used in the following places:
- Properties
- Fields
- Method Parameters
- Method Return Types
- Class Generic Type Parameters
- Method Generic Type Parameters


### AllowedTypesAttribute Examples
```csharp
public class AllowedTypesExamples
{
    /// <summary>
    /// Example of the AllowedTypes attribute being used on a property
    /// Allows only Employee and Manager types to be assigned to the property
    /// If the provided HrPerson type is used it will error with UNCT006
    /// </summary>
    [AllowedTypes<Employee,Manager>]
    public dynamic TestProperty { get; set;} // = new HrPerson("John Marks", Guid.NewGuid(), "HR", 150000, DateTime.UtcNow);   

    
    /// <summary>
    /// Example of the AllowedTypes attribute being used on a field
    /// Allows only Employee and Manager types to be assigned to the property
    /// If the provided HrPerson type is used it will error with UNCT006
    /// </summary>
    [AllowedTypes<Employee, Manager>] 
    public dynamic _Testfield; //= new HrPerson("John Marks", Guid.NewGuid(), "HR", 150000, DateTime.UtcNow);    

    
    
    /// <summary>
    /// Example of a method that uses the AllowedTypes attribute on the return type
    /// This allows the use of the dynamic keyword to return any type but applies a limit to the types that can be returned
    /// </summary>
    [return: AllowedTypes<Employee,Manager,Empty>]
    public virtual dynamic TestReturn(string name)
    {
        UnionContainer<IEmployee> _empOfMonth = Program.TryGetEmployeeOfTheMonth(name);
        if (_empOfMonth.TryGetValue() is Employee _employee)
        {
            Console.WriteLine($"Employee of the month is {_employee.Name}");
            Console.WriteLine("Thanks for all the hard work!");
            return _employee;
        }
        else if(_empOfMonth.TryGetValue() is Manager _manager)
        {
            Console.WriteLine("Congratulations to the management team!");
            Console.WriteLine($"Employee of the month is {_manager.Name}");
            return _manager;
        }
        else if(_empOfMonth.TryGetValue() is HrPerson _hrPerson)
        {
            Console.WriteLine("Sorry, HR is not eligible for employee of the month.");
            // Errors with UNCT006 for invalid allowed return type, so empty is returned instead
            //return _hrPerson; 
            return Empty.Nothing;
        }
        Console.WriteLine("type is not allowed returning empty");
        return Empty.Nothing;
    }
    
    
    /// <summary>
    /// Example of a method that uses the AllowedTypes attribute on the generic type
    /// This allows generics passed into a function to be limited to the types specified even if the class itself is not limited
    /// </summary>
    public virtual T TestGeneric<[AllowedTypes<Employee,Manager>]T>(T value)
    {
        Console.WriteLine($"value is of type {value.GetType()}");
        if (value.GetType() == typeof(Employee) || value.GetType() ==  typeof(Manager))
        {
            Console.WriteLine("Validation worked");
            return value;
        }
        Console.WriteLine($"Validation failed");
        return value;
    }
    
    
    /// <summary>
    /// Example of a method that uses the AllowedTypes attribute on the argument
    /// Allows use of the dynamic keyword for arguments while applying compile time limitations on the types that can be passed
    /// </summary>
    public virtual dynamic TestArgument([AllowedTypes<Employee,Manager>]dynamic value)
    {
        Console.WriteLine($"value is of type {value.GetType()}");
        if (value.GetType() == typeof(Employee) || value.GetType() ==  typeof(Manager))
        {
            Console.WriteLine("Validation worked");
            return value;
        }
        Console.WriteLine($"Validation failed");
        return value;
    }
}
```

- Usage examples (All examples here are from the AllowedTypesExamples class)
```csharp
public static void AllowedTypesUsageExample()
    {
        AllowedTypesExamples _typesExamples = new();
        
        //valid property assignment example
        _typesExamples.TestProperty = employee;
        //Errors with UNCT006
        _typesExamples.TestProperty = hrPerson;    
        
        //field assignment example
        _typesExamples._Testfield = employee;
        //Errors with UNCT006
        _typesExamples._Testfield = hrPerson;      
        
        //method return type example -> return type errors happen in the method being called not the method calling the method
        _typesExamples.TestReturn(employee.Name);
        //should error with UNCT006
        _typesExamples.TestReturn(newHire.Name); 
        //should error with UNCT006
        _typesExamples.TestReturn(hrPerson.Name);    

        //method allowed generics example
        _typesExamples.TestGeneric(employee);
        //Errors with UNCT006
        _typesExamples.TestGeneric(newHire);
        //Errors with UNCT006
        _typesExamples.TestGeneric(hrPerson);   
        
        //method argument example
        _typesExamples.TestArgument(employee);
        //Errors with UNCT006
        _typesExamples.TestArgument(newHire); 
        //Errors with UNCT006
        _typesExamples.TestArgument(hrPerson);
    }
```
The deny types attribute works in a similar way to the allowed types attribute but instead of allowing only the specified types it denies the specified types.

### Generic class and extension method examples
```csharp
public class UnSignedNumbersOnly<[AllowedTypes<byte, ushort, uint, ulong, nuint>] T> where T : struct, IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>, IComparisonOperators<T, T, bool>
{
    public T Number1 { get; set; }
    public T Number2 { get; set; }
    
    
    
    public T SubtractToZeroOrHigher()
    {
        T subNumber = Number1 - Number2;
        if (subNumber <= default(T))
        {
            subNumber = default(T);
        }
        return subNumber;
    }
    

    public static void OldCheckStyle(T number1 , T number2)
    {
        bool valid1 = (number1 is uint or ulong or byte or ushort or nuint);
        bool valid2 = (number2 is uint or ulong or byte or ushort or nuint);
        if (valid1 && valid2)
        {
            UnSignedNumbersOnly<T> unSignedNumbersOnly = new();
            unSignedNumbersOnly.Add(number1, number2);
        }
    }
}

public static class UnSIgnedNumberExtensions
{
    public static T Add<[DeniedTypes<ulong>]T>(this UnSignedNumbersOnly<T> unsigned, T number1, T number2) where T : struct, IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>, IComparisonOperators<T, T, bool>
    {
        return number1 + number2;
    }
}
```
In the above code block we see that the UnSignedNumbers class only allows generics of the following types `byte`, `ushort`, `uint`, `ulong`, and `nuint`.
We also see that the extension method `Add` in the `UnSignedNumberExtensions` class denies the use of the `ulong` type.

 - Usage examples
```csharp
public static void GenericUsageExample()
    {
        //valid
        UnSignedNumbersOnly<uint> unSignedints = new();
        uint number1 = 10;
        uint number2 = 5;
        Console.WriteLine(unSignedints.Add(number1, number2));
        
        //valid
        IntsAndLongsOnly<int> ints = new();
        int intNumber1 = 10;
        int intNumber2 = 5;
        Console.WriteLine(ints.Add(intNumber1, intNumber2));
        
        //Errors with UNCT006 for use of a non allowed type
        UnSignedNumbersOnly<int> signedints = new();
        signedints.Number1 = 10;
        signedints.Number2 = -5;
        Console.WriteLine(signedints.Add());
        
        
        //Errors with UNCT008 for denied type
        //The error can apply to extension methods that are in the object.ExtMethod() format and ExtensionClass.ExtMethod(object) as well
        UnSignedNumbersOnly<ulong> ulongs = new(); 
        ulong ulongNumber1 = 10;
        ulong ulongNumber2 = 5;
        Console.WriteLine(ulongs.Add(ulongNumber1, ulongNumber2));
        Console.WriteLine(UnSIgnedNumberExtensions.Add(ulongs, ulongNumber1, ulongNumber2));
    }
```
When implementing code that uses those classes like in this above example, the developer can be sure that the types used are the correct types and that the code will not compile if the wrong types are used.
The third example in the usage examples will not compile as the `int` type is not allowed in the `UnSignedNumbersOnly` class.
The fourth example will not compile as the `ulong` type is denied in the `UnSignedNumberExtensions` class.

This allows for unconstrained generics to be used in a more controlled way and can help to ensure that the correct types are used in the correct places.
It can be thought of in the same way as how OptionContains are the one of many vs tuples are the all of many, this is the same for the AllowedTypes and DeniedTypes attributes vs normal constraints where the generic must meet all the type requirements.


## Union Container Conversions 
Union Containers can be converted to other types of Union Containers as long as the target container type contains all the generic types of the source container type.
This is to ensure that the conversion is safe and that the target container can hold all the possible values of the source container.

```csharp
 UnionContainer<Employee,Manager> container = new();  
container = UnionContainerFactory.CreateWithValue(employee); 
```
The library will automatically convert the container to the correct type if the types are compatible.
To perform this sort of conversion manually, the `TryConvertContainer` method can be used.

```csharp
UnionContainer<Employee> container = new();  
UnionContainer<Employee,Manager> containerTwo = container.TryConvertContainer(typeof(UnionContainer<Employee,Manager>));
```
This allows for containers that are of a lower type to still be used as argument values, return types etc. in methods that require a higher type of container without needing to extract the value, make a new container, and return the new container.
All states are transferred to the new container so any value, error, or exception will be present in the new container.


## Global Configuration & logging
The `UnionContainers` library provides a way to configure the behavior of the library globally.

The options that can be configured are:
```csharp
 /// <summary>
    /// When true the default value will be treated as null <br/>
    /// This means container processing methods will not count the default value as a value and will be treated as empty instead <br/>
    /// Defaults to true
    /// </summary>
    public static bool TreatDefaultAsNull { get; private set; } = true;


    /// <summary>
    /// Default behavior is to set container to empty until a value is set
    /// if this is set to true the container will not be empty if there are errors or exceptions as well
    /// Defaults to true
    /// </summary>
    public static bool ContainersNotEmptyIfIssues { get; private set; } = true;


    /// <summary>
    /// When true internal exceptions that happen during container processing will be treated as errors <br/>
    /// If treated as errors they are added to the containers error state as strings <br/>
    /// Otherwise exceptions will be thrown if produced from user errors <br/>
    /// Defaults to false
    /// </summary>
    public static bool TreatExceptionsAsErrors { get; private set; } = false;
    
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
    public static bool ThrowExceptionsFromUserCode { get; private set; } = false;
```

these can be configured in the following way
```csharp
builder.Services.AddSingleton<UnionContainerFactory>();
        builder.Services.AddUnionContainerConfiguration(options =>
        {
            options.SetDefaultAsNull(true);
            options.SetContainersNotEmptyIfIssues(true);
            options.SetTreatExceptionsAsErrors(false);
            options.SetThrowExceptionsFromUserHandlingCode(false);
            options.SetLoggerOptions(logOptions =>
            {
                logOptions.SetLogger(LoggerFactory.Create(logBuilder => logBuilder.AddConsole()).CreateLogger("UnionContainerLogger"));
                logOptions.SetContainerCreationLogging(false, LogLevel.Information);
                logOptions.SetContainerConversionLogging(false, LogLevel.Information);
                logOptions.SetContainerModificationLogging(false, LogLevel.Information);
                logOptions.SetContainerResultHandlingLogging(false, LogLevel.Information);
                logOptions.SetContainerErrorHandlingLogging(false, LogLevel.Information);
            });
        });
```

## Functional Methods 
Along with the Union Containers library, there are a few generic functional methods that will help with a variety of tasks.
These can be found in the `UnionContainers.Core.Helpers.Functional` and `UnionContainers.Shared.Common.FunctionalExtensions` classes. It includes things like IsNull, IsDefault, TryCatch, IfNullDo, ForEach, etc.

They provide a functional coding style to make things like method chaining easier for example 
```csharp
public class FunctionalExtensionsDemo
{
    public static async Task TryCatchWrapper()
    {
        var httpRequestResult = await TryConnectAsync("localhost", "http", 5005).TryCatch(exception =>
        {
            Console.WriteLine("Executing custom try-catch handler: ");
            Console.WriteLine("The TryConnectAsync Method produced an exception: " + exception.Message);
        });
        
        httpRequestResult.IfNotNullDo(requestResult => Console.WriteLine($"Got back http status code {requestResult.StatusCode}"));
    }

    public static void CheckIfExample(string name = "Bob") 
        => name.CheckIf(n => n == "Bob").ThenDo(name, n => Console.WriteLine($"Name is {n}"));


    public static void CheckIfExample2(string name = "Bob")
    {
        Functional.CheckIf(name == "Bob")
        .ThenDo(() => Console.WriteLine("Name is Bob"))
        .ElseDo(() => Console.WriteLine($"Name is not Bob it is {name}"));
    }
    
    public static async Task CheckIfExample3(HttpStatusCode httpCode)
    {
        var targetHttpsStatusCode =  await ReturnRandomHttpStatusCode();
        Functional.CheckIf(targetHttpsStatusCode == httpCode)
            .ThenDo(() => Console.WriteLine("Http Status Codes match and are" + httpCode))
            .ElseDo(() => Console.WriteLine($"Http Status Codes do not match \n\t provided code: {httpCode} \n\t target code: {targetHttpsStatusCode}"));
    }

    public static void ForEachExample()
    {
        List<string?> names = new() {"Bob", null, "Joe", null, "Jane"};
        
        //typical foreach
        Console.WriteLine("Typical foreach:");
        foreach (var name in names)
        {
            Console.WriteLine(name);
        }
        Console.WriteLine();
        Console.WriteLine("ForEachIfNotNull:");
        names.ForEachIfNotNull(Console.WriteLine);
    }

    public static void ContinueWithExample()
    {
        string name = "Bob";
        int employeeId = 1;
        
        name.CheckIf(n => n.HasValue())
            .ContinueWith(name)
            .CheckIf(n => n == "Bob")
            .ThenDo(() => Console.WriteLine($"Name is {name}"))
            .ContinueWith(employeeId)
            .CheckIf(id => id > 0)
            .ThenDo(employeeId, id => Console.WriteLine("Employee Id is greater than 0"));
    }
    
    public static void ThrowIfNullExample(string? name)
    {
        name.ThrowIfNull(nameof(name))
            .ContinueWith(name)
            .CheckIf(n => n.HasValue())
            .ContinueWith(name)
            .CheckIf(n => n == "Bob")
            .ThenDo(name, n => Console.WriteLine("Name is Bob"), n => Console.WriteLine($"Name is not Bob its {n}"))
            .ContinueWith(name)
            .CheckIf(n => n == "Bob")
            .ThenDo(() => Console.WriteLine("Name is Bob"), () => Console.WriteLine($"Name is not Bob its {name}"));
    }
    
}
```


## Performance
The `UnionContainers` library is designed to be as performant as possible. With that in mind however, C# does not have native support for Union types (yet) and so the need to cast and perform type checks before getting results or executing supplied methods will have some performance impact.
The demo project contains a few performance tests that show the performance of the `UnionContainers` library compared to using a switch statement to handle the different types.
One such example using the Fody Method Time Library is shown below.
```csharp
public static void NonUserMethodToContainer()
{
    HttpClient client = new HttpClient();
    UnionContainer<HttpResponseMessage> container = new();
    container = MethodToContainer<UnionContainer<HttpResponseMessage>,HttpResponseMessage>(() => client.GetAsync("http://127.0.0.1:8080/").Result);


    container.IfEmptyDo(() => Console.WriteLine("Container is empty"))
        .TryHandleResult((HttpResponseMessage response) =>
        {
            Console.WriteLine("Container has a response: " + response);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                container.AddError($"Failed with error message {response.StatusCode}");
            }
        })
        .IfErrorDo<string>((errors) => Console.WriteLine($"Container has 1 or more errors: \n\t {errors.ToCommaSeparatedString()}"))
        .IfExceptionDo((exception) =>
        {
            Console.WriteLine("Container has an exception: " + exception.Message);
        });
}

public static void NonUserMethodComparision()
{
    HttpClient client = new HttpClient();
    HttpResponseMessage? messageResult = null;
    try
    { 
        messageResult = client.GetAsync("http://127.0.0.1:8080/").Result;
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
    
    if(messageResult is null)
    {
        Console.WriteLine("Result is empty");
    }
    else
    {
        Console.WriteLine("Http Request has a response: " + messageResult);
        if (messageResult.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine($"Failed with error message {messageResult.StatusCode}");
        }
    }
}
```
When the targeted endpoint is not listening for connections and the HttpClient throws an exception, the version with union containers completes in 2070ms, while the version without union containers completes in 2040ms.
When the targeted endpoint is listening for connections and the HttpClient returns a response, the version with union containers completes in 50ms, while the version without union containers completes in 10ms.

The performance impact of using the `UnionContainers` library is minimal and in most cases, the performance impact will be negligible. The benefits of using the `UnionContainers` library hopefully outweigh the minimal performance impact.

#### Performance Test Vs Other Libraries
`UnionContainers` uses sealed classes while `OneOf` and `LanguageExt` make use of structs which for simple situations does yield a performance increase. 
```csharp
| Method                 | Mean    | Error    | StdDev   | Allocated |
|----------------------- |--------:|---------:|---------:|----------:|
| GetHTTPResponse        | 2.031 s | 0.0084 s | 0.0079 s |  27.38 KB |
| GetHTTPResponseOneOf   | 2.029 s | 0.0087 s | 0.0081 s |  28.84 KB |
| GetHTTPResponseLangExt | 2.032 s | 0.0063 s | 0.0059 s |   28.3 KB |
| GetHTTPResponseUnion   | 2.031 s | 0.0099 s | 0.0092 s |  32.05 KB |
```
However, by being based on a class UnionContainers allows for more flexibility and ease of use than the struct-based libraries.
This includes the ability to convert between container types, to utilize the shared extension methods, and base implementations from the common abstract class.


## What are the various projects in the solution?
The `UnionContainers` solution contains the following projects:
- **UnionContainersCore** - The main project that contains the `UnionContainers` library.
- **DemoApp** - A demo project that shows how to use the `UnionContainers` library.
- **UnionContainersSourceGenerator** - A source generator that generates strongly typed containers for functions that are wrapped in the `MethodToContainer` method as well as various diagnostics for the `UnionContainers` library.
- **UnionContainerShared** - A shared project that contains shared code that is used by the `UnionContainersCore` and `UnionContainersSourceGenerator` projects.

