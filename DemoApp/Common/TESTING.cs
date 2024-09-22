using System.Runtime.InteropServices;
using UnionContainers.Containers.Base;
using UnionContainers.Containers.Standard;
using UnionContainers.Errors;
using UnionContainers.Helpers;
using HelpfulTypesAndExtensions;
using HelpfulTypesAndExtensions.BaseClasses;


namespace DemoApp.Common;

/// <summary>
/// Example class for testing
/// </summary>
public static class TESTING
{
    public static void TestMethod()
    {
        var container = new UnionContainer<int>();
        container.AddError(ClientErrors.ValidationFailure().SetMessage("This is a test error"));

        var genericContainerErrors = container.GetErrors();
        
        var validationFailureErrors = container.GetErrors<ClientErrors.ValidationFailureError>();
        
        foreach (var error in genericContainerErrors)
        {
            //match switch example 1
            (error.Type switch
            {
                {DisplayName: "Validation Failure"} => PrintError,
                {DisplayName: "Custom"} => () => Console.WriteLine($"Custom Error with message {error.Message}"),
                _ => new Action(PrintError) //this one sets the Action type so the others can be inferred
            })();
            Console.WriteLine(error.Message);
           
           
            //match switch example 2
            error.Type.Switch(ErrorType.ValidationFailure,PrintError);
           
            
            //match switch example 3
            error.Type.Switch(
                defaultAction: PrintError,
                (ErrorType.ValidationFailure, PrintError),
                (ErrorType.Custom, PrintError)
            );
            continue;


            void PrintError()
            {
              Console.WriteLine(error.GetMessage());
            }  
        }
        
        foreach (ref ClientErrors.ValidationFailureError error in CollectionsMarshal.AsSpan(validationFailureErrors))
        {
            var tempError = error;
            //match switch example 1
            (error.Type switch
            {
                {DisplayName: "Validation Failure"} => PrintError,
                _ => new Action(PrintError) //this one sets the Action type so the others can be inferred
            })();
            Console.WriteLine(error.Message);
            
            //match switch example 2
            error.Type.Switch(ErrorType.ValidationFailure, PrintError);
            
            continue;


            void PrintError()
            {
                Console.WriteLine(tempError.GetMessage());
            }  
        }
    }
    
    public static UnionContainer<int> TestMethod2(string input)
    {
        if (input.IsEmpty())
        {
            TGuid guid = new();
            return new(ClientErrors.ValidationFailure().SetMessage("Input cannot be empty"));
        }

        return UnionContainer<int>.Result(1);
    }
    
    public static UnionContainer<HttpResponseMessage> TestMethod3(string input)
    {
        if (input.IsEmpty())
        {
            return new(ClientErrors.ValidationFailure().SetMessage("Input cannot be empty"));
        }
        try
        {
            HttpClient client = new();
            var result = client.GetAsync("http://127.0.0.1:8080/").Result;
            //return new(result);
            return result;
        }
        catch (Exception e)
        {
            //return new(CustomErrors.Exception(e));
            return e;
        }
    }
    
    public static UnionContainer<int> TestMethod4()
    {
        var container = new UnionContainer<int>();
        //correctly is prevented so result value cannot be set directly
        //container.ResultValue = new ValueTuple<int>(1);
        IUnionResultContainer<ValueTuple<int>> container2 = container;
        //the internal result value cannot be accessed directly
        //container2.ResultValue = new ValueTuple<int>(1);
        return container;
    }

    public static UnionContainer<int> TestMethod5()
    {
        var container = new UnionContainer<int>();
        //correctly is prevented so state value cannot be set directly
        //container.State = UnionContainerState.Error;
        //container.Errors = new List<IError> {ClientErrors.ValidationFailure().SetMessage("This is a test error")};
        container.AddErrors(new IError[] {ClientErrors.ValidationFailure().SetMessage("This is a test error")});
        return container;
    }
    
    public static UnionContainer<Employee> TestMethod6(string name)
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
            return errors;
        }
        return new Employee(name, Guid.NewGuid(), "Employee Title", 1000, DateTime.Now);
    }

    public static void TestMethod7()
    {
        try
        {
            var container = TestMethod6("Hello World");
            //var container = TestMethod6("H"); //uncomment to test with errors
            (container.State switch
            {
                UnionContainerState.Empty => () => PrintMessage("No errors"),
                UnionContainerState.Error => () =>
                {
                    //I don't want this to be accessible, fixed via explicit interface implementation
                    //var myErrors = container.Errors; 
                    PrintMessage("Errors found");
                    container.GetErrors().ForEach(error =>
                    {
                        if (error is CustomErrors.ExceptionWrapperError e)
                        {
                            Console.WriteLine($"Oh no! An exception occurred: {e.exception}");
                        }
                        else
                        {
                            PrintMessage(error.ToRecordLikeString());
                        }
                    });
                },
                UnionContainerState.Result => () =>
                {
                    PrintMessage("Result found");
                    //var example = container.ResultValue; //I don't want this to be accessible, maybe fixed with explicit interface implementation?
                    PrintMessage($"Employee Name : {container.Match(e => e.Name)}");
                },
                _ => new Action(() => Console.WriteLine("Unknown state"))
            })();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        

        void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }
    }

    public static void TestMethod8(IUnionContainer container)
    {
        //have to use method so the internal modifier on Errors property is working
        container.GetErrors();
        //var myErrors = container.Errors; // This is correctly not accessible, however the implementing types cant prevent access??

    }
    
}
