using System.Net;
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
    public static Employee employee = new("John Doe",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow);
    public static NewHire newHire = new();
    public static HrPerson hrPerson = new("John Marks",Guid.NewGuid(), "HR", 150000, DateTime.UtcNow);
    public static HrPersonInTraining hrPersonInTraining = new("Jane Marks",Guid.NewGuid(), "HR Training", 150000, DateTime.UtcNow);
    public static Manager? manager1 = new("John Stevens",Guid.NewGuid(), "Manager", 200000, DateTime.UtcNow);
    public static ManagerInTraining managerInTraining = new("Dave Stevens",Guid.NewGuid(), "Manager Training", 200000, DateTime.UtcNow);
    public static Guid targetGuid = Guid.NewGuid();
    public static string targetName = "Mark Stevens";
    public static string targetNameTwo = "Jane Doe";
    public static string targetManagerName = "Jane Stevens";
    
    public static List<Employee> employees = new()
    {
        new("John Doe",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
        new("Jane Doe",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
        new("Bob Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
        new("Sally Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
        new("Joe Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow)
    };
    
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder();
        
        Console.WriteLine($"{Success()} Starting UnionContainer Demo Application");
        
        //configure builder
        Console.WriteLine($"{Info()} UnionContainer Supports Dependency Injection using Microsoft DI Container");
        Console.WriteLine($"{Info()} It can be configured using the AddUnionContainerConfiguration method");
        Console.WriteLine($"{Info()} It has additional support and configuration to customize logging and error handling");
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
        
        var app = builder.Build();
        
        UnionContainerConfiguration containerConfiguration = app.Services.GetRequiredService<UnionContainerConfiguration>();
        
        //start the application
        await app.StartAsync();
        
        TESTING.TestMethod7();

        /*var containerDemo = new ContainerDemo(containerConfiguration);
        var attributeDemo = new AttributeDemo();
        var functionalDemo = new FunctionalDemo();

        await containerDemo.Run();
        await attributeDemo.Run();
        await functionalDemo.Run();
        MethodTimeLogger.PrintLoggedMethodResults();*/
    }


   
    
    
    /// <summary>
    /// Method that could return a null value
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Employee? TryGetEmployeeByName(string name)
    {
        //create 5 new employees
        var employees = new List<Employee>
        {
            new Employee("John Doe",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
            new Employee("Jane Doe",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
            new Employee("Bob Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
            new Employee("Sally Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
            new Employee("Joe Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow)
        };

        //try to find the employee by name
        var employee = employees.FirstOrDefault(e => e.Name == name);
        if (employee != null)
        {
            return employee;
        }
        else
        {
            return null;
        }
    }
    
    
    /// <summary>
    /// Method that returns an UnionContainer with the employee found or an empty container if not found
    /// Also demonstrates the use of an Union Container as a parameter
    /// </summary>
    /// <param name="nameOrId"></param>
    /// <returns></returns>
    public static UnionContainer<Employee> GetEmployeeByNameOrId(UnionContainer<string, Guid> nameOrId)
    {
        //create 5 new employees
        var employees = new List<Employee>
        {
            new Employee("John Doe",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
            new Employee("Jane Doe",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
            new Employee("Bob Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
            new Employee("Sally Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
            new Employee("Joe Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow)
        };

        //uses implicit conversion to return the employee found as a container 
        return nameOrId switch
        {
            ({ } name, _) => employees.FirstOrDefault(e => e.Name == name) ?? new UnionContainer<Employee>(),
            (_, Guid id) => employees.FirstOrDefault(e => e.ID == id) ?? new UnionContainer<Employee>()
        };
    }
    

    /*public static UnionContainer<Employee> TryGetEmployeeByNameIdOrGuid(UnionContainer<string,int,Guid> nameOrId)
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
            container.AddError("No name or id provided");
        }
        Employee? employee = null;
        nameOrId.MatchResult((string name) =>
        {
            Console.WriteLine($"Trying to get employee by name {name}: searching names");
            if(name == "Bob Stevens")
            {
                container.AddError("Bob Stevens is on vacation");
                return;
            }
            employee = employees.FirstOrDefault(e => e.Name == name);
            Console.WriteLine("Finished searching for employee");
            employee.IfNullDo(() => container.AddError("No employee found with that name"));
        })
        .MatchResult((int idNumber) =>
        {
            Console.WriteLine($"Trying to get employee by id number {idNumber}: searching id values");
            employee = employees.FirstOrDefault(e => e.ID == new Guid(idNumber.ToString()));
            employee.IfNotNullDo((e) => container);
            employee.IfNullDo(() => container.AddError("No employee found with that id"));
        })
        .MatchResult((Guid id) =>
        {
            Console.WriteLine($"Trying to get employee by id {id}: searching id values");
            employee = employees.FirstOrDefault(e => e.ID == id);
            employee.IfNotNullDo((e) => container);
            employee.IfNullDo(() => container.AddError("No employee found with that id"));
        });
        return container;
    }*/
    

    public static Employee? TryGetEmployeeByNameOrIdWithoutContainers(string? name = null, int? id = null, Guid? guid= null)
    {
        //create 5 new employees
        var employees = new List<Employee>
        {
            new Employee("John Doe",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
            new Employee("Jane Doe",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
            new Employee("Bob Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
            new Employee("Sally Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow),
            new Employee("Joe Stevens",Guid.NewGuid(), "Manager", 100000, DateTime.UtcNow)
        };
        Employee? employee = null;
        if(name != null)
        {
            Console.WriteLine($"Trying to get employee by name {name}: searching names");
            employee = employees.FirstOrDefault(e => e.Name == name);
            if(employee == null)
            {
                Console.WriteLine("No employee found with that name");
            }
        }
        if(id != null)
        {
            Console.WriteLine($"Trying to get employee by id number {id}: searching id values");
            employee = employees.FirstOrDefault(e => e.ID == new Guid(id.ToString()));
            if(employee == null)
            {
                Console.WriteLine("No employee found with that id");
            }
        }
        if(guid != null)
        {
            Console.WriteLine($"Trying to get employee by id {guid}: searching id values");
            employee = employees.FirstOrDefault(e => e.ID == guid);
            if(employee == null)
            {
                Console.WriteLine("No employee found with that id");
            }
        }
        return employee;
    }
    
    
    /// <summary>
    /// Similar to the above method but returns an UnionContainer with either an employee or a manager
    /// </summary>
    /// <param name="nameOrId"></param>
    /// <returns></returns>
    public static UnionContainer<Employee,Manager> GetEmployeeOrManagerByNameOrId(UnionContainer<string, Guid> nameOrId)
    {
        //create 5 new employees
        var employees = new List<Employee>
        {
            new Employee("John Doe",Guid.NewGuid(), "Employee", 100000, DateTime.UtcNow),
            new Employee("Jane Doe",Guid.NewGuid(), "Employee", 100000, DateTime.UtcNow),
            new Employee("Bob Doe",Guid.NewGuid(), "Employee", 100000, DateTime.UtcNow),
            new Employee("Sally Doe",Guid.NewGuid(), "Employee", 100000, DateTime.UtcNow),
            new Employee("Joe Doe",Guid.NewGuid(), "Employee", 100000, DateTime.UtcNow)
        };
        
        var managers = new List<Manager>
        {
            new Manager("John Stevens",Guid.NewGuid(), "Manager", 150000, DateTime.UtcNow),
            new Manager("Jane Stevens",Guid.NewGuid(), "Manager", 150000, DateTime.UtcNow),
            new Manager("Bob Stevens",Guid.NewGuid(), "Manager", 150000, DateTime.UtcNow),
            new Manager("Sally Stevens",Guid.NewGuid(), "Manager", 150000, DateTime.UtcNow),
            new Manager("Joe Stevens",Guid.NewGuid(), "Manager", 150000, DateTime.UtcNow)
        };
        
        Console.WriteLine("Getting employee or manager by name or id");
        if (nameOrId is ({ } thename, _))
        {
            Console.WriteLine($"Name: {thename}");
        }
        else if (nameOrId is (_, Guid id))
        {
            Console.WriteLine($"ID: {id}");
        }
        else
        {
            Console.WriteLine("No name or id given");
        }

        return nameOrId switch
        {
            ({ } name, _) => employees.FirstOrDefault(e => e.Name == name) ?? managers.FirstOrDefault(m => m.Name == name) ?? new UnionContainer<Employee,Manager>(),
            (_, Guid id) => employees.FirstOrDefault(e => e.ID == id) ?? managers.FirstOrDefault(m =>m.ID == id) ?? new UnionContainer<Employee,Manager>()
        };
    }

    public static UnionContainer<IEmployee> TryGetEmployeeOfTheMonth(string name)
    {
        if(name == "John Doe")
        {
            return employee;
        }
        if(name == "John Stevens")
        {
            return manager1;
        }
        if(name == "John Marks")
        {
            return hrPerson;
        }
        return new();
    }
    
    public static HttpResponseMessage TryConnectLocalhost(string connectionAddress, string protocol, int port)
    {
        //make a http request to localhost:5000, wrapping the task in a TryOption
        HttpClient client = new HttpClient();
        var result = client.GetAsync($"{protocol}://{connectionAddress}:{port}/").Result;
        return result;
    }
    
    public static async Task<HttpResponseMessage> TryConnectAsync(string connectionAddress, string protocol, int port)
    {
        //make a http request to localhost:5000, wrapping the task in a TryOption
        HttpClient client = new HttpClient();
        var result = await client.GetAsync($"{protocol}://{connectionAddress}:{port}/");
        return result;
    }
    
    public static async Task<HttpStatusCode> ReturnRandomHttpStatusCode()
    {
        await Task.Delay(1000);
        Random random = new Random();
        //return a random value from the HttpStatusCode enum
        return Enum.GetValues<HttpStatusCode>().IfNotNullDo(values => values![random.Next(values.Length)]);
    }
}