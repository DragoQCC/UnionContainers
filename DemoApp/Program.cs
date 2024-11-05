using System.Net;
using System.Net.Http.Headers;
using DemoApp.Common;
using HelpfulTypesAndExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UnionContainers;
using static DemoApp.Common.ConsoleMessageHelpers;

namespace DemoApp;

public class Program
{
    // demo components
    public static Programmer programmer = new("John Doe", Guid.NewGuid(),DateTime.UtcNow);
    public static NewHire newHire = new();
    public static HrPerson hrPerson = new("John Marks", Guid.NewGuid(),DateTime.UtcNow);
    public static HrPersonInTraining hrPersonInTraining = new("Jane Marks", Guid.NewGuid(),DateTime.UtcNow);
    public static Manager? manager1 = new("John Stevens", Guid.NewGuid(),DateTime.UtcNow);
    public static ManagerInTraining managerInTraining = new("Dave Stevens",Guid.NewGuid(),DateTime.UtcNow);
    public static Guid targetGuid = Guid.NewGuid();
    public static string targetName = "Mark Stevens";
    public static string targetNameTwo = "Jane Doe";
    public static string targetManagerName = "Jane Stevens";

    public static List<IEmployee> employees = new()
    {
        new Programmer("John Doe", Guid.NewGuid(),DateTime.UtcNow),
        new Programmer("Jane Doe", Guid.NewGuid(),DateTime.UtcNow),
        new Manager("Bob Stevens", Guid.NewGuid(),DateTime.UtcNow),
        new Manager("Sally Stevens", Guid.NewGuid(),DateTime.UtcNow),
        new Manager("Joe Stevens", Guid.NewGuid(),DateTime.UtcNow)
    };

    private static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        Console.WriteLine($"{Success()} Starting UnionContainer Demo Application");

        //configure builder
        Console.WriteLine($"{Info()} UnionContainer Supports Dependency Injection using Microsoft's DI Container");
        Console.WriteLine($"{Info()} It can be configured using the AddUnionContainerConfiguration method");
        Console.WriteLine($"{Info()} It has additional support and configuration to customize logging and error handling");
        builder.Services.AddSingleton<UnionContainerFactory>();
        
        builder.Services.AddUnionContainerConfiguration(config =>
        {
            config.AddDefaultErrorConverters();
            config.TryRegisterErrorHandler<ClientErrors.InvalidOperationError>(new ErrorHandler<ClientErrors.InvalidOperationError>((error) => Console.WriteLine($"Invalid Operation Error: {error.Message}")));
        });

        IHost app = builder.Build();
        //start the application
        await app.StartAsync();
        await TESTING.Start();
    }





    /// <summary>
    ///     Method that could return a null value
    /// </summary>
    /// <param name="name"> </param>
    /// <returns> </returns>
    public static Programmer? TryGetEmployeeByName(string name)
    {
        //create 5 new employees
        var employees = new List<Programmer>
        {
            new("John Doe", Guid.NewGuid(),DateTime.UtcNow),
            new("Jane Doe", Guid.NewGuid(), DateTime.UtcNow),
            new("Bob Stevens", Guid.NewGuid(), DateTime.UtcNow),
            new("Sally Stevens", Guid.NewGuid(), DateTime.UtcNow),
            new("Joe Stevens", Guid.NewGuid(), DateTime.UtcNow)
        };

        //try to find the employee by name
        Programmer? employee = employees.FirstOrDefault(e => e.Name == name);
        if (employee != null)
        {
            return employee;
        }

        return null;
    }
    
    /// <summary>
    ///     Method that returns an UnionContainer with the employee found or an empty container if not found
    ///     Also demonstrates the use of an Union Container as a parameter
    /// </summary>
    /// <param name="nameOrId"> </param>
    /// <returns> </returns>
    public static UnionContainer<Programmer> GetEmployeeByNameOrId(UnionContainer<string, Guid> nameOrId)
    {
        //create 5 new employees
        var employees = new List<Programmer>
        {
            new("John Doe", Guid.NewGuid(),DateTime.UtcNow),
            new("Jane Doe", Guid.NewGuid(),DateTime.UtcNow),
            new("Bob Stevens", Guid.NewGuid(),DateTime.UtcNow),
            new("Sally Stevens", Guid.NewGuid(),DateTime.UtcNow),
            new("Joe Stevens", Guid.NewGuid(),DateTime.UtcNow)
        };

        //uses implicit conversion to return the employee found as a container 
        return nameOrId switch
        {
            ({
            } name, _) => employees.FirstOrDefault(e => e.Name == name) ?? new UnionContainer<Programmer>(),
            (_, Guid id) => employees.FirstOrDefault(e => e.ID == id) ?? new UnionContainer<Programmer>()
        };
    }
    
    public static Programmer? TryGetEmployeeByNameOrIdWithoutContainers(string? name = null, int? id = null, Guid? guid = null)
    {
        //create 5 new employees
        var employees = new List<Programmer>
        {
            new("John Doe", Guid.NewGuid(),DateTime.UtcNow),
            new("Jane Doe", Guid.NewGuid(),DateTime.UtcNow),
            new("Bob Stevens", Guid.NewGuid(),DateTime.UtcNow),
            new("Sally Stevens", Guid.NewGuid(),DateTime.UtcNow),
            new("Joe Stevens", Guid.NewGuid(),DateTime.UtcNow)
        };

        Programmer? employee = null;
        if (name != null)
        {
            Console.WriteLine($"Trying to get employee by name {name}: searching names");
            employee = employees.FirstOrDefault(e => e.Name == name);
            if (employee == null)
            {
                Console.WriteLine("No employee found with that name");
            }
        }

        if (id != null)
        {
            Console.WriteLine($"Trying to get employee by id number {id}: searching id values");
            employee = employees.FirstOrDefault(e => e.ID == new Guid(id.ToString()));
            if (employee == null)
            {
                Console.WriteLine("No employee found with that id");
            }
        }

        if (guid != null)
        {
            Console.WriteLine($"Trying to get employee by id {guid}: searching id values");
            employee = employees.FirstOrDefault(e => e.ID == guid);
            if (employee == null)
            {
                Console.WriteLine("No employee found with that id");
            }
        }

        return employee;
    }
    
    /// <summary>
    ///     Similar to the above method but returns an UnionContainer with either an employee or a manager
    /// </summary>
    /// <param name="nameOrId"> </param>
    /// <returns> </returns>
    public static UnionContainer<Programmer,Manager> GetEmployeeOrManagerByNameOrId(UnionContainer<string, Guid> nameOrId)
    {
        //create 5 new employees
        var programmers = new List<Programmer>
        {
            new("John Doe", Guid.NewGuid(),DateTime.UtcNow),
            new("Jane Doe", Guid.NewGuid(),DateTime.UtcNow),
            new("Bob Doe", Guid.NewGuid(),DateTime.UtcNow),
            new("Sally Doe", Guid.NewGuid(),DateTime.UtcNow),
            new("Joe Doe", Guid.NewGuid(),DateTime.UtcNow)
        };

        var managers = new List<Manager>
        {
            new("John Stevens", Guid.NewGuid(),DateTime.UtcNow),
            new("Jane Stevens", Guid.NewGuid(),DateTime.UtcNow),
            new("Bob Stevens", Guid.NewGuid(),DateTime.UtcNow),
            new("Sally Stevens", Guid.NewGuid(),DateTime.UtcNow),
            new("Joe Stevens", Guid.NewGuid(),DateTime.UtcNow)
        };

        Console.WriteLine("Getting employee or manager by name or id");

        return nameOrId.Match
        (
            name => programmers.FirstOrDefault(e => e.Name == name) ?? managers.FirstOrDefault(m => m.Name == name) ?? new UnionContainer<Programmer,Manager>(ResourceErrors.NotFound($"No employee or manager found with name {name}")),
            id => programmers.FirstOrDefault(e => e.ID == id) ?? managers.FirstOrDefault(m => m.ID == id) ?? new UnionContainer<Programmer,Manager>(ResourceErrors.NotFound($"No employee or manager found with id {id}")),
            () => new(ServerErrors.Unexpected("No name or id provided"))
        );
    }

    public static UnionContainer<IEmployee> TryGetEmployeeOfTheMonth(string name)
    {
        if (name == "John Doe")
        {
            return programmer;
        }

        if (name == "John Stevens")
        {
            return manager1;
        }

        if (name == "John Marks")
        {
            return hrPerson;
        }

        return new UnionContainer<IEmployee>();
    }

    public static HttpResponseMessage TryConnectLocalhost(string connectionAddress, string protocol, int port)
    {
        //make a http request to localhost:5000, wrapping the task in a TryOption
        var client = new HttpClient();
        HttpResponseMessage result = client.GetAsync($"{protocol}://{connectionAddress}:{port}/").Result;
        return result;
    }

    public static async Task<HttpResponseMessage> TryConnectAsync(string connectionAddress, string protocol, int port)
    {
        //make a http request to localhost:5000, wrapping the task in a TryOption
        var client = new HttpClient();
        HttpResponseMessage result = await client.GetAsync($"{protocol}://{connectionAddress}:{port}/");
        return result;
    }

    public static async Task<HttpStatusCode> ReturnRandomHttpStatusCode()
    {
        await Task.Delay(1000);
        var random = new Random();
        //return a random value from the HttpStatusCode enum
        return Enum.GetValues<HttpStatusCode>().IfNotNullDo(values => values![random.Next(values.Length)]);
    }
}