/*using System;
using DemoApp.Common;
using UnionContainers.Containers.DefaultError;
using UnionContainers.Helpers;
using UnionContainers.Shared.Common;

namespace DemoApp.TypeLimitingAttributeExamples;

public class AllowedTypesExamples
{
    /// <summary>
    /// Example of the AllowedTypes attribute being used on a property
    /// Allows only Employee and Manager types to be assigned to the property
    /// If the provided HrPerson type is used it will error with UNCT006
    /// </summary>
    [AllowedTypes<Employee,Manager>]
    public dynamic TestProperty { get; set;} // = new HrPerson("John Marks", Guid.NewGuid(), "HR", 150000, DateTime.UtcNow);   

    
    /// <summary>
    /// Example of the AllowedTypes attribute being used on a field
    /// Allows only Employee and Manager types to be assigned to the property
    /// If the provided HrPerson type is used it will error with UNCT006
    /// </summary>
    [AllowedTypes<Employee, Manager>] 
    public dynamic _Testfield; //= new HrPerson("John Marks", Guid.NewGuid(), "HR", 150000, DateTime.UtcNow);    

    
    
    /// <summary>
    /// Example of a method that uses the AllowedTypes attribute on the return type
    /// This allows the use of the dynamic keyword to return any type but applies a limit to the types that can be returned
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [return: AllowedTypes<Employee,Manager,Empty>]
    public virtual dynamic TestReturn(string name)
    {
        UnionContainer<IEmployee> _empOfMonthContainer = Program.TryGetEmployeeOfTheMonth(name);

        var employeeResult = _empOfMonthContainer.MatchResult((Employee employee) =>
        {
            Console.WriteLine($"Employee of the month is {employee.Name}");
            Console.WriteLine("Thanks for all the hard work!");
            return employee;
        }).GetMatchedItemAs<Employee>();
        if(employeeResult != null)
        {
            return employeeResult;
        }
        var managerResult = _empOfMonthContainer.MatchResult((Manager manager) =>
        {
            Console.WriteLine("Congratulations to the management team!");
            Console.WriteLine($"Employee of the month is {manager.Name}");
            return manager;
        }).GetMatchedItemAs<Manager>();
        if(managerResult != null)
        {
            return managerResult;
        }
        var hrPersonResult = _empOfMonthContainer.MatchResult((HrPerson hrPerson) =>
        {
            Console.WriteLine("Sorry, HR is not eligible for employee of the month.");
            return Empty.Nothing;
        }).GetMatchedItemAs<Empty>();
        if(hrPersonResult != null)
        {
            return hrPersonResult;
        }
        Console.WriteLine("No employee of the month found. Returning empty.");
        return Empty.Nothing;
    }
    
    
    /// <summary>
    /// Example of a method that uses the AllowedTypes attribute on the generic type
    /// This allows generics passed into a function to be limited to the types specified even if the class itself is not limited
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public virtual T TestGeneric<[AllowedTypes<Employee,Manager>]T>(T value)
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
    /// Example of a method that uses the AllowedTypes attribute on the argument
    /// Allows use of the dynamic keyword for arguments while applying compile time limitations on the types that can be passed
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual dynamic TestArgument([AllowedTypes<Employee,Manager>]dynamic value)
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
}*/