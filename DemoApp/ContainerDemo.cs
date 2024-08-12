using System.Security.Cryptography;
using DemoApp.Common;
using DemoApp.ContainerCreationExamples;
using DemoApp.ContainerResultMatchExamples;
using MethodTimer;
using UnionContainers.Core.Common;
using UnionContainers.Core.Helpers;
using UnionContainers.Core.UnionContainers;
using static DemoApp.Program;
using static DemoApp.Common.ConsoleMessageHelpers;

namespace DemoApp;

public class ContainerDemo(UnionContainerConfiguration containerConfiguration)
{
    public async Task Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("-------------------------Union Container Demo-----------------------------------");
        Console.WriteLine($"{Info()} UnionContainerConfiguration.DefaultAsNull: {containerConfiguration.UnionContainerOptions.DefaultAsNull}");
        Console.WriteLine($"{Info()} UnionContainerConfiguration.ContainerEmptyIfIssues: {containerConfiguration.UnionContainerOptions.ContainersNotEmptyIfIssues}");
        Console.WriteLine($"{Info()} UnionContainerConfiguration.TreatExceptionsAsErrors: {containerConfiguration.UnionContainerOptions.TreatExceptionsAsErrors}");
        Console.WriteLine($"{Info()} UnionContainerConfiguration.ThrowExceptionsFromUserHandlingCode: {containerConfiguration.UnionContainerOptions.ThrowExceptionsFromUserHandlingCode}");
        Console.WriteLine();
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("------------------Union Container Intro & Basic Match Examples------------------");
        try
        {
            ContainerHelloWorldIntro();
            Console.WriteLine();
            BasicMatchExample();
            Console.WriteLine();
            BasicNonContainerMatchExample();
            Console.WriteLine();
            ContainerDefaultAsNullDemo(containerConfiguration);
            Console.WriteLine();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("------------------Explicit Container Creation Examples--------------------------");
        try
        {
            //Constructor example
            var exampleContainer = new UnionContainer<string, int, Guid>(Guid.NewGuid());
            ExplicitContainerCreation.NonUserMethodToContainer();
            Console.WriteLine();
            ExplicitContainerCreation.NonUserMethodComparision();
            Console.WriteLine();
            ExplicitContainerCreation.UserFunctionWrapperExample();
            Console.WriteLine();
            ExplicitContainerCreation.ToUnionContainerExample();
            Console.WriteLine();
            ExplicitContainerCreation.ContainerFromMethodExample();
            Console.WriteLine();
            await ExplicitContainerCreation.UserTaskWrapperExample();
            Console.WriteLine();
            await ExplicitContainerCreation.NonUserTaskToContainer();
            Console.WriteLine();
            await ExplicitContainerCreation.NonUserTaskToContainerCancelExample(new CancellationTokenSource());
            Console.WriteLine();
            ExplicitContainerCreation.JsonTest();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        Console.WriteLine("Example of a container being used to wrap unsafe methods such as divide by zero");
        var divideByZeroResuiult1 = ExplicitContainerCreation.DivideByZeroTestContainer();
        divideByZeroResuiult1.TryHandleResult((int value) => Console.WriteLine("Result of divide by zero: " + value));
        divideByZeroResuiult1.IfExceptionDo(ex => Console.WriteLine("Exception occurred: " + ex.Message));
        try
        {
            Console.WriteLine();
            Console.WriteLine("Example of just a try-catch being used to handle divide by zero");
            var divideByZeroResult2 = ExplicitContainerCreation.DivideByZeroTest();
            Console.WriteLine("Result of divide by zero: " + divideByZeroResult2);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception occurred: " + e.Message);
        }
        
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("------------------Implicit Container Creation Examples--------------------------");
        try
        {
            ImplicitContainerCreation.ImplicitContainerCreationExample();
            ImplicitContainerCreation.ImplicitContainerCreationExampleTwo();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("------------------Container Issue Checking Examples-----------------------------");
        try
        {
            IssuesMatch.ReturnIssueFromInvokedMethod();
            IssuesMatch.IfIssuesMatch();
            IssuesMatch.HasIssuesSingleMatchExample();
            IssuesMatch.HasIssuesExample();
            IssuesMatch.AddErrorsAndGetExample();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        try
        {
            IssuesMatch.AddErrorsAndGetExample2();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("------------------Container Result Match Examples-------------------------------");
        try
        {
            ResultMatch.DeconstructionMatch();
            ResultMatch.IsResultExamples();
            UnionContainer<string> containerToPassOn = ResultMatch.TryGetValueUnknownContainer(new UnionContainer<string,int>(5));
            ResultMatch.TryGetValueKnownContainer(containerToPassOn);
            ResultMatch.HandleResultExamples();
            Console.WriteLine();
            ResultMatch.TryGetValueFallbacks();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("------------------Container Result Match and Extract Examples------------------");
        try
        {
            MatchAndExtract.ContainerSingleHandleValueExtract();
            Console.WriteLine();
            MatchAndExtract.ContainerMultiHandleValueExtract();
            Console.WriteLine();
            MatchAndExtract.ContainerMultiHandleUniqueTypeExtraction();
            Console.WriteLine();
            MatchAndExtract.ExampleMethod2();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
    }
    
    
    
    
    [Time("Basic Union Container match example that errors")]
    private void BasicMatchExample()
    {
        TryGetEmployeeByNameIdOrGuid("Bob Stevens")
            .TryHandleResult((Employee resultItem) =>
            {
                Console.WriteLine("Container has a result");
                Console.WriteLine($"Employee found: {resultItem.Name}");
            })
            .IfEmptyDo(() =>
            {
                Console.WriteLine("no user, Container is empty");
            })
            .IfErrorDo<string>(errorValues =>
            {
                Console.WriteLine("Error occurred while getting employee");
                Console.WriteLine("Error values:");
                foreach (var errorValue in errorValues)
                {
                    Console.WriteLine("\t" + errorValue);
                }
            })
            .IfExceptionDo(ex =>
            {
                Console.WriteLine("Exception occurred: " + ex.Message);
            });
    }
    
    [Time("Basic Example Without Union Container")]
    private void BasicNonContainerMatchExample()
    {
        try
        {
            var employee = TryGetEmployeeByNameOrIdWithoutContainers("Bob Stevens");
            if (employee != null)
            {
                Console.WriteLine("Employee found: " + employee.Name);
                if(employee.Name == "Bob Stevens")
                {
                    Console.WriteLine("Bob stevens is on vacation");
                }
            }
            else
            {
                Console.WriteLine("No user found");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception occurred: " + e.Message);
        }
    }

    private static void ContainerDefaultAsNullDemo(UnionContainerConfiguration containerConfiguration)
    {
        Console.WriteLine($"{Info()} Showcase of UnionContainerConfiguration.DefaultAsNull being set to {containerConfiguration.UnionContainerOptions.DefaultAsNull}");
        UnionContainer<string,int> container2 = new();
        Console.WriteLine($"{Info()} Setting Container Value to the default value of the type");
        if (containerConfiguration.UnionContainerOptions.DefaultAsNull)
        {
            container2.SetValue(0);
            Console.WriteLine($"{Info()} When Default is treated like null the container will not handle the result as it is the default value of the type");
            container2.TryHandleResult((int value) => Console.WriteLine("Container value is: " + value)); //skipped
        }
        else
        {
            container2.SetValue(0);
            Console.WriteLine($"{Info()} When Default is treated like a value the container will handle the result as it is not null");
            container2.TryHandleResult((int value) => Console.WriteLine("Container value is: " + value)); // executes
        }
        container2.SetValue(5);
        container2.TryHandleResult((int value) => Console.WriteLine("Container value is: " + value));
    }

    private static void ContainerHelloWorldIntro()
    {
        Console.WriteLine($"{Info()} Creating basic UnionContainer by assigning a value to it");
        Console.WriteLine($"{Info()} When assigning a value directly a implicit conversion is used to assign the value to the container");
        UnionContainer<string> container = "Hello World";
        Console.WriteLine($"{Success()} UnionContainer created with a string value");
        Console.WriteLine($"{Caution()} Once a container is made with a specific type it can only be assigned values of that type or derived types");
        
        //Produces error UNCT001, a container cannot be assigned a type that is not in the container / a derived type
        Console.WriteLine($"{Error()} Uncomment the below line to see the build/design time error produced when trying to assign a value of a different type to the container");
        //container.SetValue(7.5);
        Console.WriteLine($"{Info()} When working with a container the TryHandleResult method can be used to handle the result of the container");
        Console.WriteLine($"{Info()} TryHandleMethod supports handling the result of the container with a Action, Func, or Task, it also supports passing in methods to handle exceptions & fallback values when a Func or value returning Task is used");
        Console.WriteLine($"{Success()} In this example a simple Action/Lambda calling Console.WriteLine was passed in to handle the result of the container");
        container.TryHandleResult((string value) => Console.WriteLine("\t The value of the container is: " + value));
        
        Console.WriteLine($"{Info()} On a single type container the TryGetValue method can be used to get the value of the container");
        string? containerValue = container.TryGetValue();
        Console.WriteLine($"Container value: {containerValue}");
        Console.WriteLine();
        Console.WriteLine($"{Caution()} It is possible that the container does not hold a value which will cause the method to return null");
        Console.WriteLine($"{Info()} It is safer to use the TryHandleResult method as it will ignore the result if it is null, meaning your passed in method will never receive a null value");
    }
}