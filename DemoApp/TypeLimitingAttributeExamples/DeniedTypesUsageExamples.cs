/*using System;

namespace DemoApp.TypeLimitingAttributeExamples;
using static DemoApp.Program;

public class DeniedTypesUsageExamples
{
    public static void DeniedTypesUsageExample()
    {
        DeniedTypesExamples _typesExamples = new();
        
        //property assignment example
        _typesExamples.TestProperty = employee;
        //Errors with UNCT008
        //_typesExamples.TestProperty = hrPerson;    
        
        //field assignment example
        _typesExamples._Testfield = employee;
        //Errors with UNCT008
        //_typesExamples._Testfield = hrPerson;      
        
        //method return type example
        Console.WriteLine("TestReturn method example with valid type");
        _typesExamples.TestReturn(employee.Name);
        //should error with UNCT008 -> for returns the error happens inside the method so it will not be caught here
        Console.WriteLine("TestReturn method example with unchecked invalid type");
        _typesExamples.TestReturn(newHire.Name); 
        //should error with UNCT008
        Console.WriteLine("TestReturn method example with invalid type");
        _typesExamples.TestReturn(hrPerson.Name);    

        //method Denied generics example
        Console.WriteLine("TestGeneric method example with valid type");
        _typesExamples.TestGeneric(newHire);
        
        //Errors with UNCT008
        //_typesExamples.TestGeneric(employee);
        //Errors with UNCT008
        //_typesExamples.TestGeneric(manager1);   
        
        //method argument example
        Console.WriteLine("TestArgument method example with valid type");
        _typesExamples.TestArgument(employee);
        //Errors with UNCT008
        //_typesExamples.TestArgument(newHire); 
        //Errors with UNCT008
        //_typesExamples.TestArgument(hrPerson);
    }
}*/