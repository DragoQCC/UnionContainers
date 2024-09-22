/*using System;
using DemoApp.Common;
using UnionContainers.Containers.DefaultError;
using UnionContainers.Helpers;
using UnionContainers.Shared.Common;

namespace DemoApp.TypeLimitingAttributeExamples;


public class DeniedTypesExamples
{
    /// <summary>
    /// Example of the DeniedTypes attribute being used on a property
    /// Allows only Employee and Manager types to be assigned to the property
    /// If the provided HrPerson type is used it will error with UNCT008
    /// </summary>
    [DeniedTypes<Employee,Manager>]
    public dynamic TestProperty { get; set;} // = new HrPerson("John Marks", Guid.NewGuid(), "HR", 150000, DateTime.UtcNow);   

    
    /// <summary>
    /// Example of the DeniedTypes attribute being used on a field
    /// Allows only Employee and Manager types to be assigned to the property
    /// If the provided HrPerson type is used it will error with UNCT008
    /// </summary>
    [DeniedTypes<Employee, Manager>] 
    public dynamic _Testfield; //= new HrPerson("John Marks", Guid.NewGuid(), "HR", 150000, DateTime.UtcNow);    

    
    
    /// <summary>
    /// Example of a method that uses the DeniedTypes attribute on the return type
    /// This allows the use of the dynamic keyword to return any type but applies a limit to the types that can be returned
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [return: DeniedTypes<Employee,Manager>]
    public virtual dynamic TestReturn(string name)
    {
        bool shortCircuit = false;
        UnionContainer<IEmployee> _empOfMonthContainer = Program.TryGetEmployeeOfTheMonth(name);

        _empOfMonthContainer.MatchResult((Employee employee) =>
        {
            Console.WriteLine("Sorry, normal employees is not eligible for employee of the month.");
            shortCircuit = true;
        }).MatchResult((Manager manager) =>
        {
            Console.WriteLine("Sorry, management is not eligible for employee of the month.");
            shortCircuit = true;
        });
        
        if (shortCircuit is false)
        {
            var hrPersonResult = _empOfMonthContainer.MatchResult((HrPerson hrPerson) =>
            {
                Console.WriteLine("Congratulations to the HR team!");
                Console.WriteLine($"Employee of the month is {hrPerson.Name}");
                return hrPerson;
            });
            if(hrPersonResult != null)
            {
                return hrPersonResult;
            }
        }
        Console.WriteLine("No employee of the month found. Returning empty.");
        return Empty.Nothing;
    }
    
    
    
    
    
    /// <summary>
    /// Example of a method that uses the DeniedTypes attribute on the generic type
    /// This allows generics passed into a function to be limited to the types specified even if the class itself is not limited
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public virtual T TestGeneric<[DeniedTypes<Employee,Manager>]T>(T value)
    {
        Console.WriteLine($"value is of type {value.GetType()}");
        if (value.GetType() == typeof(Employee) || value.GetType() ==  typeof(Manager))
        {
            Console.WriteLine("Validation worked");
            return value;
        }
        Console.WriteLine($"Validation failed");
        return value;
    }
    
    
    /// <summary>
    /// Example of a method that uses the DeniedTypes attribute on the argument
    /// Allows use of the dynamic keyword for arguments while applying compile time limitations on the types that can be passed
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual dynamic TestArgument([DeniedTypes<Employee,Manager>]dynamic value)
    {
        Console.WriteLine($"value is of type {value.GetType()}");
        if (value.GetType() == typeof(Employee) || value.GetType() ==  typeof(Manager))
        {
            Console.WriteLine("Validation worked");
            return value;
        }
        Console.WriteLine($"Validation failed");
        return value;
    }
    
    
    /// <summary>
    /// Example of using a class that has a denied type attribute on a generic for its Add method
    /// The add method will allow any type except for the denied type ex.(long,ulong)
    /// </summary>
    public static void DeniedTypesUsageExample()
    {
        Console.WriteLine("Denied Types Usage Example");
        CustomNumberAddition _customNumberAddition = new();
        long number1 = 10;
        long number2 = 5;
        //gives a UNCT008 error for denied type usage
        //_customNumberAddition.Add(number1, number2);
       
        //errors with UNCT008 for denied type
        //CustomNumberAddition.StaticAdd(number1, number2);
        
        //valid because the type is not denied
        int number3 = 10;
        int number4 = 5;
        Console.WriteLine("Showcase where the denied type is not used and the method is called successfully");
        _customNumberAddition.Add(number3, number4);
    }
}*/