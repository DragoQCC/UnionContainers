using System.Net;
using UnionContainers.Core.Helpers;
using UnionContainers.Shared.Common;
using static DemoApp.Program;


namespace DemoApp.FunctionalExtensionsExamples;


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