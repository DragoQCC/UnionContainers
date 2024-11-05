using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using HelpfulTypesAndExtensions;
using UnionContainers;

namespace DemoApp.Common;

/// <summary>
///     Example class for testing
/// </summary>
public static class TESTING
{
    static List<string> EmployeeNames = ["John Doe", "Jane Doe", "Bob Stevens", "Sally Stevens", "Joe Stevens"];
    
    public static void TestMethod()
    {
        string myString = "Hello World";
        //myString.EqualsCaseInsensitive("hello world");
        
        var container = new UnionContainer<int>();
        container.AddError(ClientErrors.ValidationFailure("This is a test error"));

        List<IError> genericContainerErrors = container.GetErrors();

        List<ClientErrors.ValidationFailureError> validationFailureErrors = container.GetErrors<ClientErrors.ValidationFailureError>();

        foreach (IError error in genericContainerErrors)
        {
            //match switch example 1
            (error.Type switch
            {
                {
                    DisplayName: "Validation Failure"
                } => PrintError,
                {
                    DisplayName: "Custom"
                } => () => Console.WriteLine($"Custom Error with message {error.Message}"),
                _ => new Action(PrintError) //this one sets the Action type so the others can be inferred
            })();

            Console.WriteLine(error.Message);


            //match switch example 2
            error.Type.Switch(ErrorType.ValidationFailure, PrintError);


            //match switch example 3
            error.Type.Switch(PrintError, (ErrorType.ValidationFailure, PrintError), (ErrorType.Custom, PrintError));
            continue;


            void PrintError()
            {
                Console.WriteLine(error.GetMessage());
            }
        }

        foreach (ref ClientErrors.ValidationFailureError error in CollectionsMarshal.AsSpan(validationFailureErrors))
        {
            ClientErrors.ValidationFailureError tempError = error;
            //match switch example 1
            (error.Type switch
            {
                {
                    DisplayName: "Validation Failure"
                } => PrintError,
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
            return new UnionContainer<int>(ClientErrors.ValidationFailure("Input cannot be empty"));
        }
        return UnionContainer<int>.ResultContainer(1);
    }

    public static UnionContainer<HttpResponseMessage> TestMethod3(string input)
    {
        if (input.IsEmpty())
        {
            return new UnionContainer<HttpResponseMessage>(ClientErrors.ValidationFailure("Input cannot be empty"));
        }

        try
        {
            HttpClient client = new();
            HttpResponseMessage result = client.GetAsync("http://127.0.0.1:8080/").Result;
            return result;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public static UnionContainer<int> TestMethod4()
    {
        var container = new UnionContainer<int>();
        //correctly is prevented so result value cannot be set directly
        //container.ResultValue = new ValueTuple<int>(1);
        IUnionContainerResult<ValueTuple<int>> container2 = container;
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
        container.AddErrors
        (
            new IError[]
            {
                ClientErrors.ValidationFailure("This is a test error")
            }
        );

        return container;
    }

    public static UnionContainer<Programmer> TestMethod6(string name)
    {
        List<IError> errors = [];
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
            return errors;
        }
        return new Programmer(name, Guid.NewGuid(),DateTime.UtcNow);
    }

    public static void TestMethod7()
    {
        try
        {
            //var container = TestMethod6("Hello World");
            UnionContainer<Programmer> container = TestMethod6("H"); //uncomment to test with errors


            //container.IfEmptyDo(() => PrintMessage("No errors or result found"));

            (container.GetState() switch
            {
                UnionContainerState.Empty => () => PrintMessage("No errors"),
                UnionContainerState.Error => () =>
                {
                    //I don't want this to be accessible, fixed via explicit interface implementation
                    //var myErrors = container.Errors; 
                    PrintMessage("Errors found");
                    container
                        .GetErrors()
                        .ForEach
                        (
                            error =>
                            {
                                if (error is CustomErrors.ExceptionWrapperError e)
                                {
                                    Console.WriteLine($"Oh no! An exception occurred: {e.exception}");
                                }
                                else
                                {
                                    PrintMessage(error.ToRecordLikeString());
                                }
                            }
                        );
                },
                UnionContainerState.Result => () =>
                {
                    PrintMessage("Result found");
                    //var example = container.ResultValue; //I don't want this to be accessible, maybe fixed with explicit interface implementation?
                    PrintMessage($"Employee Name : {container.Match(e => e.Name, () => "")}");
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
        // This is correctly not accessible, fixed with explicit interface implementation and internal modifier
        //var myErrors = container.Errors; 
        //have to use method so the internal modifier on Errors property is working
        container.GetErrors();
    }

    public static void TestMethod9(UnionContainer<string,Guid> nameorId)
    {
        UnionContainer<Programmer, Manager> container = Program.GetEmployeeOrManagerByNameOrId(nameorId);
        var result = container.Match(
            programmer => programmer.Name,
            manager =>  manager.Name,
            () => "No result was returned");
        
        Console.WriteLine(result);
    }
    
    public static void TestMethod10(UnionContainer<string,Guid> nameorId)
    {
        UnionContainer<Programmer, Manager> container = Program.GetEmployeeOrManagerByNameOrId(nameorId);
        
        (string bonusReason,int bonus) = container.Match(
            programmer =>
            {
                int programmerBonus = 1000;
                if (programmer.Name is "John Doe")
                {
                    return ("John Doe is being promoted to manager soon, up his bonus to match",(programmerBonus *2));
                }
                return ("Programmers get a standard bonus of ",programmerBonus);
            }, manager =>
            {
                if (manager.StartDate < (DateTime.Now - TimeSpan.FromDays(30)))
                {
                    return ("Recently promoted managers receive a reduced bonus",2000);
                }
                return ("Managers in the role longer then 30 days, are entitled to a full managerial bonus",5000);
            },
            () => ("Employee is not a programmer or manager",500));
        
        Console.WriteLine($"The bonus is ${bonus} USD, because {bonusReason}");
    }

    public static UnionContainer<string> GetNameIfNameInList(UnionContainer<string> name)
    {
        //In this example onErrors, and onException are commentted out because we are not expecting any errors or exceptions to be passed into this method
        //however they can be uncommented and used if needed
        return name.Match
        (
            onResult: matchedName => EmployeeNames.Contains(matchedName) ? new UnionContainer<string>(matchedName) : new UnionContainer<string>(ResourceErrors.NotFound("No employee found with that name")), 
            onNoResult: () => new UnionContainer<string>(new ArgumentException("Name cannot be null or empty"))
            //onError: e => new UnionContainer<string>(e),
            //onException: ex => new UnionContainer<string>(ex)
        );
    }
    
    
    public static async Task<UnionContainer<HttpResponseMessage>> HttpRequestExample(string url)
    {
        var client = new HttpClient();
        return await UnionContainer<HttpResponseMessage>.MethodToContainer(() => client.GetAsync(url));
    }

    public static async Task<UnionContainer<HttpResponseMessage>> HttpRequestExampleAdvanced(string url)
    {
        var client = new HttpClient();
        var requestContainer = await UnionContainer<HttpResponseMessage>.MethodToContainer(() => client.GetAsync(url));
        return requestContainer.Match
        (
            message =>
            {
                if (message.IsSuccessStatusCode)
                {
                    return new UnionContainer<HttpResponseMessage>(message);
                }

                IError error = message.StatusCode switch
                {
                    HttpStatusCode.BadRequest => NetworkErrors.GenericNetworking($"Request to {url} is invalid"),
                    HttpStatusCode.Unauthorized => ClientErrors.Unauthorized($"Request to {url} is missing login credentials"),
                    HttpStatusCode.Forbidden => ClientErrors.Forbidden($"Users request to {url} does not have sufficient access rights and was blocked"),
                    HttpStatusCode.NotFound => ClientErrors.NotFound($"The requested url {url} was not found"),
                    HttpStatusCode.InternalServerError => ServerErrors.GenericFailure($"The requested endpoint at {url} returned an internal error"),
                    _ => NetworkErrors.GenericNetworking($"Request to {url} failed with status code {message.StatusCode} and reason {message.ReasonPhrase}")
                };

                return new UnionContainer<HttpResponseMessage>(error);
            }, 
            () => new UnionContainer<HttpResponseMessage>(CustomErrors.Custom($"The request to {url}, returned null")), 
            onException: (exception) =>
            {
                switch (exception)
                {
                    case TaskCanceledException tcex : return new UnionContainer<HttpResponseMessage>(NetworkErrors.Timeout($"The request to {url} timed out: {tcex.Message}"));
                    case UriFormatException uriEx : return new UnionContainer<HttpResponseMessage>(ClientErrors.ValidationFailure($"The requested URI {url} is invalid: {uriEx.Message}"));
                    case HttpRequestException httpRequestEx :
                    {
                        var networkingError = NetworkErrors.GenericNetworking($"The request to {url} failed with status code {httpRequestEx.StatusCode} and reason {httpRequestEx.Message}");
                        networkingError.SetSource(httpRequestEx.Source);
                        return new UnionContainer<HttpResponseMessage>(networkingError);
                    }
                    case InvalidOperationException invalidOperationEx : return new UnionContainer<HttpResponseMessage>(invalidOperationEx);
                    default: return new UnionContainer<HttpResponseMessage>(exception);
                }
            }
        );
    }

    public static double MyLegacyDivideMethod(double num1, int num2)
    {
        return num1 / num2;
    }

    public static char? ProcessFinalGradeLegacy(List<double> quarterGrades)
    {
        try
        {
            if (quarterGrades.Count == 0)
            {
                throw new ArgumentException($"The number of grades must be higher then 0 for {nameof(quarterGrades)}");
            }
            double quarterTotal = 0.0;
            foreach (float grade in quarterGrades)
            {
                quarterTotal += grade;
            }
            var quarterAverage = MyLegacyDivideMethod(quarterTotal, quarterGrades.Count);
            return quarterAverage switch
            {
                <= 100 and >= 90 => 'A',
                <= 89 and >= 80 => 'B',
                <= 79 and >= 70 => 'C',
                <= 70 and >= 60 => 'D',
                <= 60 and >= 50 => 'F',
                _ => null
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public static UnionContainer<char> ProcessFinalGrade(List<double> quarterGrades)
    {
        if (quarterGrades.Count == 0)
        {
            return new UnionContainer<char>(ClientErrors.ValidationFailure($"The number of grades must be higher than 0 for {nameof(quarterGrades)}"));
        }
        double quarterTotal = 0.0;
        foreach (float grade in quarterGrades)
        {
            quarterTotal += grade;
        }
        var quarterAverageContainer = UnionContainer<double>.MethodToContainer(() => MyLegacyDivideMethod(quarterTotal, quarterGrades.Count));
        return quarterAverageContainer.Match(quarterAverage =>
        {
            char? letterGrade = quarterAverage switch
            {
                <= 100 and >= 90 => 'A',
                <= 89 and >= 80 => 'B',
                <= 79 and >= 70 => 'C',
                <= 70 and >= 60 => 'D',
                <= 60 and >= 50 => 'F',
                _ => null
            };
            return letterGrade.FromNullableToContainer();
        },
        () => new UnionContainer<char>(CustomErrors.Custom("Failed to process grades")));
    }


    public static UnionContainer<int> ExceptionToErrorTest()
    {
        try
        {
            throw new InvalidOperationException("This is a test exception");
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public static async Task Start()
    {
        string test = "Hello World";
        test.EqualsCaseInsensitive("my Other string");
        
        ExceptionToErrorTest()
            .Match
            (
                number => Console.WriteLine($"The number is {number}"), 
                () => Console.WriteLine("No number was returned")
                //errors => errors.ForEachIfNotNull(error => Console.WriteLine($"\t{error}"))
            );
        TestMethod();
        TestMethod2("Hello");
        TestMethod3("Hello");
        TestMethod4();
        var method5Container = TestMethod5();
        if (method5Container.GetState() is UnionContainerState.Error)
        {
            Console.WriteLine("Container 5 has errors");
            method5Container.GetErrors().ForEachIfNotNull(error => Console.WriteLine($"\t {error}"));
        }
        TestMethod6("Hello World");
        TestMethod7();
        TestMethod8(new UnionContainer<int>());
        UnionContainer<string> resultName = GetNameIfNameInList("John Doe");
        resultName.Match
        (
            onResult: name => Console.WriteLine($"Employee found with name {name}"),
            onNoResult: () => Console.WriteLine("No employee found with that name"),
            onErrors: errors => errors!.ForEachIfNotNull(e => Console.WriteLine($"\t{e.GetName()}: {e.Message}")),
            onException: ex => Console.WriteLine(ex.Message)
        );
        var httpSimpleContainer = await HttpRequestExample("http://localhost:5000/");
        httpSimpleContainer.Match
        (
            responseMessage => Console.WriteLine($"Http Request was successful, with response message {responseMessage}"),
            () => Console.WriteLine("Get Request returned null, no exception or errors"),
            onException: async exception =>
            {
                if (exception is TaskCanceledException)
                {
                    Console.WriteLine("Waiting 5 seconds before retrying request");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    var secondRequest = await HttpRequestExample("http://localhost:5000/");
                    Console.WriteLine($"Status of second request is {secondRequest.GetState()}");
                }
            }
        );

        try
        {
            double result = MyLegacyDivideMethod(5, 0);
            Console.WriteLine($"Result of division is {result}");
        }
        catch (Exception e)
        {
            Console.WriteLine("No result was returned");
            if(e is DivideByZeroException)
            {
                Console.WriteLine("Cannot divide by zero, make sure the second number is not zero");
            }
            else
            {
                Console.WriteLine(e);
            }
            throw;
        }

        var doubleDivideContainer = UnionContainer<double>.MethodToContainer(() => MyLegacyDivideMethod(5, 0));
        doubleDivideContainer.Match
        (
            result => Console.WriteLine($"Result of division is {result}"),
            () => Console.WriteLine("No result was returned"),
            onErrors: errors => errors.ForEachIfNotNull(error => Console.WriteLine($"\t{error}"))
        );

        List<double> grades = [];
        try
        {
            var letterGrade = ProcessFinalGradeLegacy(grades);
            Console.WriteLine($"Letter grade is {letterGrade}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        var finalGradeContainer = ProcessFinalGrade(grades);
        finalGradeContainer.Match
        (
            letterGrade => Console.WriteLine($"Letter grade is {letterGrade}"), 
            () => Console.WriteLine($"Failed to process grades, please double check list"),
            errors => errors.ForEachIfNotNull(error => Console.WriteLine($"\t{error}"))
        );


    }
}