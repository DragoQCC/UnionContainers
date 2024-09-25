namespace DemoApp.TypeLimitingAttributeExamples;

public class GenericClassUsageExamples
{
    /// <summary>
    /// Example of using a class with a generic that has an allowed type attribute
    /// it also applied a denied type attribute to the generic extension method "Add"
    /// </summary>
    public static void GenericUsageExample()
    {
        //Errors with UNCT006 for use of a non allowed type
        /*
        UnSignedNumbersOnly<int> signedints = new();
        signedints.Number1 = 10;
        signedints.Number2 = -5;
        Console.WriteLine(signedints.Add());
        */
        
        /*Errors with UNCT008 for denied type
        The error can apply to extension methods that are in the object.ExtMethod() format and ExtensionClass.ExtMethod(object) as well*/
        /*UnSignedNumbersOnly<ulong> ulongs = new(); 
        ulong ulongNumber1 = 10;
        ulong ulongNumber2 = 5;
        Console.WriteLine(ulongs.Add(ulongNumber1, ulongNumber2));
        Console.WriteLine(UnSIgnedNumberExtensions.Add(ulongs, ulongNumber1, ulongNumber2));*/
        
        //valid
        Console.WriteLine("example showing allowing Unsigned numbers only");
        UnSignedNumbersOnly<uint> unSignedints = new();
        uint number1 = 10;
        uint number2 = 5;
        Console.WriteLine(unSignedints.Add(number1, number2));
        
        //valid
        Console.WriteLine("example showing allowing Ints and Longs only");
        IntsAndLongsOnly<int> ints = new();
        int intNumber1 = 10;
        int intNumber2 = 5;
        Console.WriteLine(ints.Add(intNumber1, intNumber2));
    }
    
}