namespace DemoApp.TypeLimitingAttributeExamples;
using static DemoApp.Program;

public class AllowedTypesUsageExamples
{
    public static void AllowedTypesUsageExample()
    {
        AllowedTypesExamples _typesExamples = new();
        
        //property assignment example
        _typesExamples.TestProperty = employee;
        //Errors with UNCT006
        //_typesExamples.TestProperty = hrPerson;    
        
        //field assignment example
        _typesExamples._Testfield = employee;
        //Errors with UNCT006
        //_typesExamples._Testfield = hrPerson;      
        
        //method return type example
        Console.WriteLine("TestReturn method example with valid type");
        _typesExamples.TestReturn(employee.Name);
        //should error with UNCT006 -> for returns the error happens inside the method so it will not be caught here
        Console.WriteLine("TestReturn method example with unchecked invalid type");
        _typesExamples.TestReturn(newHire.Name); 
        //should error with UNCT006
        Console.WriteLine("TestReturn method example with invalid type");
        _typesExamples.TestReturn(hrPerson.Name);    

        //method allowed generics example
        Console.WriteLine("TestGeneric method example with valid type");
        _typesExamples.TestGeneric(employee);
        //Errors with UNCT006
        //_typesExamples.TestGeneric(newHire);
        //Errors with UNCT006
        //_typesExamples.TestGeneric(hrPerson);   
        
        //method argument example
        Console.WriteLine("TestArgument method example with valid type");
        _typesExamples.TestArgument(employee);
        //Errors with UNCT006
        //_typesExamples.TestArgument(newHire); 
        //Errors with UNCT006
        //_typesExamples.TestArgument(hrPerson);
    }
}