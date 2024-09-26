using DemoApp.Common;
using UnionContainers;
using static DemoApp.Program;

namespace DemoApp.ContainerResultMatchExamples;

public class MatchAndExtract
{
    /// <summary>
    ///     Fluent match example for UnionContainer
    ///     showcases responses for handling the various error states and results
    ///     Uses the HandleResult method to handle the specific container result types
    /// </summary>
    public static void ContainerSingleHandleValueExtract()
    {
        UnionContainer<Programmer, Manager> container = GetEmployeeOrManagerByNameOrId("Jane Doe");

        Console.WriteLine("Example of a container matching the correct type & returning a result for the testValue variable");
        string containerExtractedName = container
            .MatchResult
            (
                (Programmer employee) =>
                {
                    Console.WriteLine($"Found employee returning name: {employee.Name}");
                    return employee.Name;
                }, exception =>
                {
                    Console.WriteLine($"Error occurred: {exception.Message}");
                    return "exception";
                }, "none"
            )
            .GetMatchedItemAs<string>()!;

        string containerExtractedName3 = container
            .MatchResult
            (
                (Programmer employee) =>
                {
                    Console.WriteLine($"Found employee returning name: {employee.Name}");
                    return employee.Name;
                }, fallbackValue: "none"
            )
            .GetMatchedItemAs<string>()!;

        Console.WriteLine("Example of a container matching a different type, but having a fallback value set for the testValue2 variable");
        int containerExtractSalary = container
            .MatchResult
            (
                (Manager manager) =>
                {
                    Console.WriteLine($"Found employee returning salary: {manager.Salary}");
                    return manager.Salary;
                }, fallbackValue: 200000
            )
            .GetMatchedItemAs<int>();

        Console.WriteLine($"Extracted a string from the container: {containerExtractedName}");
        Console.WriteLine($"Extracted an int from the container: {containerExtractSalary}");
    }


    /// <summary>
    ///     Fluent match example for UnionContainer
    ///     showcases responses for handling the various error states and results
    ///     Uses the HandleResult method to handle the specific container result types
    /// </summary>
    public static void ContainerMultiHandleValueExtract()
    {
        UnionContainer<Programmer, Manager> container = GetEmployeeOrManagerByNameOrId("Jane Doe");

        //Goal here is that we want to extract the name of the employee or manager & set that as the value of the container
        //Otherwise we just have an Empty container which is fine to ignore
        UnionContainer<string> nameContainer =
            container.MatchResult((Programmer employee) => employee.Name).MatchResult((Manager manager) => manager.Name).GetMatchedItemAs<string>()!;


        UnionContainer<string> nameContainer2 = (string)container
            .MatchResult((Manager manager) => manager.Name)
            .MatchResult((Programmer employee) => employee.Name)
            .GetMatchedItem()!;


        nameContainer.IfEmptyDo(() => Console.WriteLine("Name container is empty")).MatchResult(name => Console.WriteLine($"Name Container Value: {name}"));

        nameContainer2.IfEmptyDo(() => Console.WriteLine("Name container 2 is empty")).MatchResult(name => Console.WriteLine($"Name Container 2 Value: {name}"));
    }

    public static void ContainerMultiHandleUniqueTypeExtraction()
    {
        UnionContainer<Programmer, Manager> container = GetEmployeeOrManagerByNameOrId("Jane Stevens");
        container.MatchResult((Programmer employee) => employee.Name).MatchResult((Manager manager) => manager.Salary);

        UnionContainer<string, int> nameOrSalaryContainer = new();
        nameOrSalaryContainer.SetValue(container.GetMatchedItemAs<string>()).SetValue(container.GetMatchedItemAs<int>());

        nameOrSalaryContainer.MatchResult((int salary) => Console.WriteLine($"Manager Salary: {salary}"));
        nameOrSalaryContainer.MatchResult((string name) => Console.WriteLine($"Employee Name: {name}"));
        ExampleMethod();
    }



    public static void ExampleMethod()
    {
        //Create a container
        UnionContainer<string, int> container = new UnionContainer<string, int>().SetValue(5);
        //this will print "The value is an int 5"
        container.MatchResult((int value) => Console.WriteLine($"The value is an int {value}"), (string value) => Console.WriteLine($"The value is a string {value}"));
    }

    public static void ExampleMethod2()
    {
        //Create a container
        UnionContainer<string, int> container = new UnionContainer<string, int>().SetValue(5);

        var number1 = container.TryGetValue(fallbackValue: 1);
        Console.WriteLine($"The value is {number1}"); // prints 5


        container.MatchResult
        (
            (int value) =>
            {
                Console.WriteLine($"The value is an int that when added to 5 equals {value + 5}");
                return value + 5;
            }
        );

        int number2 = container.GetMatchedItemAs<int>();
        Console.WriteLine($"The value is {number2}"); // prints 10
    }
}