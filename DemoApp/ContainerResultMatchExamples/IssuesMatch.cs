using DemoApp.Common;
using HelpfulTypesAndExtensions;
using UnionContainers;
using static DemoApp.Program;

namespace DemoApp.ContainerResultMatchExamples;

public class IssuesMatch
{
    /// <summary>
    ///     Method that demonstrates how to check if a container has issues
    ///     A container has issues if it is empty, has an error, or has an exception
    ///     An optional HandleIssues method can be used to handle the issues taking Actions as arguments
    /// </summary>
    public static void HasIssuesExample()
    {
        UnionContainer<Programmer> container = TryGetEmployeeByName(targetNameTwo);
        container.SetException(new Exception("An exception occurred"));
        if (!container.HasIssues())
        {
            return;
        }

        Console.WriteLine("Container one has issues");
        container
            .IfEmptyDo(() => Console.WriteLine("Container one is empty"))
            .HandleIssues<string>
            (
                isError: errors => Console.WriteLine($"Container one has an error \n error values: {errors.ToCommaSeparatedString()}"),
                isException: exception => Console.WriteLine("Container one has an exception: " + exception.Message)
            );
    }

    public static Empty ReturnIssueFromInvokedMethod()
    {
        try
        {
            TryGetEmployeeByNameIdOrGuid("Bob Stevens")
                .MatchResult
                (
                    (Programmer resultItem) =>
                    {
                        Console.WriteLine("Container has a result");
                        Console.WriteLine($"Employee found: {resultItem.Name}");
                    }
                )
                .IfEmptyDo(() => { Console.WriteLine("no user, Container is empty"); })
                .IfErrorDo<string>
                (
                    errorValues =>
                    {
                        Console.WriteLine("Error occurred while getting employee");
                        Console.WriteLine("Error values:");
                        foreach (var errorValue in errorValues)
                        {
                            Console.WriteLine("\t" + errorValue);
                        }
                    }
                )
                .IfExceptionDo(ex => { Console.WriteLine("Exception occurred: " + ex.Message); });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return Empty.Nothing;
    }

    /// <summary>
    ///     Method that demonstrates how to check if a container has issues
    ///     A container has issues if it is empty, has an error, or has an exception
    ///     Instead of the HandleIssues method, the IsEmpty, HasErrors, and HasException methods can be used to check the state of the container
    ///     Not all states need to be accounted for in the code if they are not relevant
    /// </summary>
    public static void HasIssuesSingleMatchExample()
    {
        UnionContainer<Programmer> container = TryGetEmployeeByName(targetNameTwo);
        container.AddError("An error occurred").AddError("A second error occurred");
        if (container.HasIssues())
        {
            Console.WriteLine("Container two has issues");
            if (container.HasErrors())
            {
                Console.WriteLine($"Container two has an error \n error values: {container.GetErrors<string>().ToCommaSeparatedString()}");
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
    ///     Method showing the use of the fluent method chaining to check container status and execute the provided actions
    /// </summary>
    public static void IfIssuesMatch()
    {
        UnionContainer<Programmer> container = TryGetEmployeeByName("Not real");
        container
            .IfEmptyDo(() => Console.WriteLine("Container is empty"))
            .IfErrorDo<string>(errors => Console.WriteLine($"Container has an error \n error values: {errors.ToCommaSeparatedString()}"))
            .IfExceptionDo(exception => Console.WriteLine("Container has an exception: " + exception.Message));
    }

    public static void AddErrorsAndGetExample()
    {
        UnionContainer<int> container = ThingThatMightError(42);
        if (container.HasErrors())
        {
            Console.WriteLine($"Container3 has 1 or more errors: {container.GetErrors<string>().ToCommaSeparatedString()}");
        }
    }

    public static void AddErrorsAndGetExample2()
    {
        UnionContainer<Guid, int> container = ThingThatMightError2("bob");
        if (container.HasErrors())
        {
            Console.WriteLine($"Container4 has 1 or more errors: {container.GetErrors().ToCommaSeparatedString()}");
        }
    }

    private static UnionContainer<int> ThingThatMightError(int badNumber)
    {
        UnionContainer<int> container = badNumber;
        if (container.TryGetValue() is 42)
        {
            container.AddErrors<string>(Status.Rejected.ToString(), "Container cannot be 42", DateTime.Now.ToString());
        }

        return container;
    }


    private static UnionContainer<Guid, int> ThingThatMightError2(string name)
    {
        UnionContainer<Guid, int> container = new();
        if (name == "bob")
        {
            container.AddErrors(Status.Rejected, "Bob is not an employee", DateTime.Now);
        }
        else if (name == "joe")
        {
            container.SetValue(42);
        }
        else
        {
            container.SetValue(Guid.NewGuid());
        }

        return container;
    }
}