/*using System;
using UnionContainers.Helpers;
using System.Net;
using System.Net.Http;
using DemoApp.Common;
using UnionContainers.Containers.Base;
using UnionContainers.Containers.DefaultError;
using static DemoApp.Program;
using static UnionContainers.Helpers.UnionContainerFactory;

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
    public static UnionContainer<string> TryGetValueUnknownContainer(IUnionContainer<> container)
    {
        UnionContainer<string> resultContainer = new();
        try
        {
            //getting the value of the container
            var containerResult = container.TryGetValue();
            Console.WriteLine($"container result was of type {containerResult?.GetType()} and value {containerResult}");
            if (containerResult is string stringResult)
            {
                resultContainer.SetValue(stringResult);
            }
            else
            {
                resultContainer.AddError("Failed to get a value");
            }
        }
        catch (Exception e)
        {
            resultContainer.SetException(e);
        }
        return resultContainer;
    }
    
    public static void TryGetValueKnownContainer(UnionContainer<string> container)
    {
        //getting the value of the container
        try
        {
            var containerResult = container.TryGetValue(fallbackValue: "No value found")!;
            Console.WriteLine($"container result {containerResult}");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to get a value");
            if(container.HasErrors())
            {
                Console.WriteLine(container.GetErrors());
            }
            if(container.HasException())
            {
                Console.WriteLine(container.GetException());
            }
        }
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

    public static void TryGetValueFallbacks()
    {
        UnionContainer<string> container = new();
        string containerValue = container.TryGetValue(fallbackValue: "No value found")!;
        Console.WriteLine("Container value: " + containerValue);

        string containerValue2 = container.TryGetValue(fallbackValueMethod: () => 
        {
            Console.WriteLine("No value found, please enter a value");
            return Console.ReadLine();
        })!;
        Console.WriteLine("Container value2: " + containerValue2);
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
        
        var container2 = MethodToContainer<string,HttpStatusCode,HttpResponseMessage>(() =>
        {
            HttpClient client = new HttpClient();
            var result = client.GetAsync("http://127.0.0.1:8080/").Result;
            if (result.StatusCode != HttpStatusCode.OK)
            {
                return result.StatusCode;
            }
            else if (result.StatusCode == HttpStatusCode.OK)
            {
                return result;
            }
            return result.Content.ReadAsStringAsync().Result;
        });

        container.MatchResult(
            (string responseBody) => Console.WriteLine("Got a response of: " + responseBody),
            (HttpStatusCode statuscode) => Console.WriteLine("Got a response status code of : " + statuscode));
        
        UnionContainer<Employee, ManagerInTraining> containerTwo = employee;
        containerTwo.IfEmptyDo(() => Console.WriteLine("Container is empty"))
            .MatchResult((Employee employee) => Console.WriteLine($"Container value is an employee \n\t{employee}"))
            .MatchResult((ManagerInTraining manager) => Console.WriteLine($"Container value is a manager \n\t {manager}"));
    }
}*/