using System.Net;
using System.Text.Json;
using DemoApp.Common;
using MethodTimer;
using UnionContainers;
using static DemoApp.Common.ConsoleMessageHelpers;
using static DemoApp.Program;


namespace DemoApp.ContainerCreationExamples;

public class ExplicitContainerCreation
{
    public static void ToUnionContainerExample()
    {
        Console.WriteLine("ToContainer Example");
        UnionContainer<Programmer> container = TryGetEmployeeByName(targetNameTwo).ToContainer();
    }

    public static void ContainerFromMethodExample()
    {
        // direct assignment but uses the implicit conversion operator for the argument
        Console.WriteLine("ContainerFromMethod Example");
        UnionContainers.Containers.Standard.UnionContainer<Programmer, Manager> container = GetEmployeeOrManagerByNameOrId(targetManagerName);
    }


    /// <summary>
    ///     Example of using the MethodToContainer method to wrap a method that returns a value
    ///     In this example it is a method that the user does not control and would have the ability to ensure it would properly handle exceptions, produce errors, etc.
    ///     The user would also be incapable of modifying the source of the method to return an UnionContainer but the MethodToContainer method allows the user to safely execute the method and handle the result
    /// </summary>
    [Time("HTTP request, with container")]
    public static void NonUserMethodToContainer()
    {
        var client = new HttpClient();
        UnionContainer<HttpResponseMessage> container = new();
        container = MethodToContainer<UnionContainer<HttpResponseMessage>, HttpResponseMessage>(() => client.GetAsync("http://127.0.0.1:8080/").Result);


        container
            .IfEmptyDo(() => Console.WriteLine("Container is empty"))
            .MatchResult
            (
                (HttpResponseMessage response) =>
                {
                    Console.WriteLine("Container has a response: " + response);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        container.AddError($"Failed with error message {response.StatusCode}");
                    }
                }
            )
            .IfErrorDo<string>(errors => Console.WriteLine($"Container has 1 or more errors: \n\t {errors.ToCommaSeparatedString()}"))
            .IfExceptionDo(exception => { Console.WriteLine("Container has an exception: " + exception.Message); });
    }

    [Time("HTTP request, no container")]
    public static void NonUserMethodComparision()
    {
        var client = new HttpClient();
        HttpResponseMessage? messageResult = null;
        try
        {
            messageResult = client.GetAsync("http://127.0.0.1:8080/").Result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        if (messageResult is null)
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

    public static async Task NonUserTaskToContainer()
    {
        var client = new HttpClient();
        UnionContainer<HttpResponseMessage> container = await MethodToContainer(async () => await client.GetAsync("http://127.0.0.1:8081/"));

        container
            .IfEmptyDo(() => Console.WriteLine("Container is empty"))
            .MatchResult
            (
                (HttpResponseMessage response) =>
                {
                    Console.WriteLine("Container has a response: " + response);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        container.AddError($"Failed with error message {response.StatusCode}");
                    }
                }
            )
            .IfErrorDo<string>(errors => Console.WriteLine($"Container has 1 or more errors: \n\t {errors.ToCommaSeparatedString()}"))
            .IfExceptionDo(exception => { Console.WriteLine("Container has an exception: " + exception.Message); });
    }

    public static async Task NonUserTaskToContainerCancelExample(CancellationTokenSource tokenSource)
    {
        var client = new HttpClient();
        var containerTask = MethodToContainer(async () => await client.GetAsync("http://127.0.0.1:8082/", tokenSource.Token));

        Task cancelTask = Task.Run
        (
            async () =>
            {
                await Task.Delay(2000);
                await tokenSource.CancelAsync();
            }
        );

        Task results = await Task.WhenAny(containerTask, cancelTask);
        UnionContainer<HttpResponseMessage> container = results == containerTask
            ? await containerTask
            : new UnionContainer<HttpResponseMessage>();

        container
            .IfEmptyDo(() => Console.WriteLine("Container is empty"))
            .MatchResult
            (
                (HttpResponseMessage response) =>
                {
                    Console.WriteLine("Container has a response: " + response);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        container.AddError($"Failed with error message {response.StatusCode}");
                    }
                }
            )
            .IfErrorDo<string>(errors => Console.WriteLine($"Container has 1 or more errors: \n\t {errors.ToCommaSeparatedString()}"))
            .IfExceptionDo(exception => { Console.WriteLine("Container has an exception: " + exception.Message); });
    }


    /// <summary>
    ///     Example where an existing user function is passed into the MethodToContainer method this allows the safe execution of the method and the handling of the result without needing to modify existing source
    /// </summary>
    public static void UserFunctionWrapperExample()
    {
        var container = MethodToContainer<UnionContainer<HttpResponseMessage>, HttpResponseMessage>(() => TryConnectLocalhost("localhost", "http", 5005));

        container
            .IfEmptyDo(() => Console.WriteLine("Container is empty"))
            .MatchResult
            (
                (HttpResponseMessage result) =>
                {
                    Console.WriteLine("processing result");
                    HttpStatusCode returnValue = GetConnectionResponseCode(result);
                    Console.WriteLine("Finished processing result return value: " + returnValue);
                    if (returnValue != HttpStatusCode.OK)
                    {
                        container.AddError(returnValue);
                    }
                }
            )
            .IfErrorDo<HttpStatusCode>(errors => Console.WriteLine($"Container has an error: \n\t {errors.ToCommaSeparatedString()}"))
            .IfExceptionDo(exception => Console.WriteLine("Container has an exception: " + exception.Message));
    }


    public static void ContainerConversionExample()
    {
        var container = MethodToContainer<UnionContainer<HttpResponseMessage>, HttpResponseMessage>(() => TryConnectLocalhost("localhost", "http", 5005));

        container.TryConvertContainer<UnionContainers.Containers.Standard.UnionContainer<int, string>>();

        container
            .IfEmptyDo(() => Console.WriteLine("Container is empty"))
            .MatchResult((HttpResponseMessage result) => Console.WriteLine("processing result"))
            .IfErrorDo<HttpStatusCode>(errors => Console.WriteLine($"Container has an error: \n\t {errors.ToCommaSeparatedString()}"))
            .IfExceptionDo(exception => Console.WriteLine("Container has an exception: " + exception.Message));
    }



    public static async Task UserTaskWrapperExample()
    {
        var container = await MethodToContainer(async () => await TryConnectAsync("localhost", "http", 5005));

        container
            .IfEmptyDo(() => Console.WriteLine("Container is empty"))
            .MatchResult
            (
                (HttpResponseMessage result) =>
                {
                    Console.WriteLine("processing result");
                    HttpStatusCode returnValue = GetConnectionResponseCode(result);
                    Console.WriteLine("Finished processing result return value: " + returnValue);
                    if (returnValue != HttpStatusCode.OK)
                    {
                        container.AddError(returnValue);
                    }
                }
            )
            .IfErrorDo<HttpStatusCode>(errors => Console.WriteLine($"Container has an error: \n\t {errors}"))
            .IfExceptionDo(exception => Console.WriteLine("Container has an exception: " + exception.Message));
    }

    public static HttpStatusCode GetConnectionResponseCode(HttpResponseMessage response)
    {
        Console.WriteLine("response: " + response);
        return response.StatusCode;
    }

    public static void JsonTest()
    {
        Console.WriteLine($"{Info()} Json Serialization 6 union value container Example");
        //create a container of all the job related types in the demo app and set its value state
        UnionContainer<Programmer, NewHire, Manager, ManagerInTraining, HrPerson, HrPersonInTraining> bigContainer = new();
        bigContainer.SetValue(programmer);

        //json serialization of the big container
        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        string jsonContainer = JsonSerializer.Serialize(bigContainer, options);
        Console.WriteLine($"{Info()} Json: \n" + jsonContainer);


        //deserialize the json back into a container
        var bigContainer2 = JsonSerializer.Deserialize<UnionContainer<Programmer, NewHire, Manager, ManagerInTraining, HrPerson, HrPersonInTraining>>(jsonContainer);
        Console.WriteLine($"{Info()} If at least one of the handlers matched the container values type the passed in method will be executed");
        bigContainer2.MatchResult
        (
            (Programmer result) => Console.WriteLine("Big container2 value: " + result), (NewHire result) => Console.WriteLine("Big container2 value: " + result),
            (Manager result) => Console.WriteLine("Big container2 value: " + result), (ManagerInTraining result) => Console.WriteLine("Big container2 value: " + result),
            (HrPerson result) => Console.WriteLine("Big container2 value: " + result), (HrPersonInTraining result) => Console.WriteLine("Big container2 value: " + result)
        );


        //Produces error UNCT009 for each invalid type in the handler list
        Console.WriteLine("Example of invalid handler types being used");
        bigContainer2.MatchResult
        (
            (int result) => Console.WriteLine("Big container2 value: " + result), (string result) => Console.WriteLine("Big container2 value: " + result),
            (char result) => Console.WriteLine("Big container2 value: " + result), (bool result) => Console.WriteLine("Big container2 value: " + result),
            (DateTime result) => Console.WriteLine("Big container2 value: " + result), (List<string> result) => Console.WriteLine("Big container2 value: " + result)
        );
    }

    public static UnionContainer<int> DivideByZeroTestContainer()
    {
        var container = new UnionContainer<int>();
        container = MethodToContainer(() => RandomExampleMethods.Divide(10, 0));
        return container;
    }

    public static void Test()
    {
        UnionContainer<int> container = UnionContainerFactory.Create(10);
    }

    public static int DivideByZeroTest()
    {
        int result = RandomExampleMethods.Divide(10, 0);
        return result;
    }
}