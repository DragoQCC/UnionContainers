using System.Numerics;
using UnionContainers.Shared.Common;

namespace DemoApp.TypeLimitingAttributeExamples;


/// <summary>
/// The following class is an example of how to use the AllowedTypes attribute to limit the types that can be used in a generic class
/// When constructing the UnSignedNumbersOnly class, the generic type T can only be of type byte, ushort, uint, ulong, or nuint
/// The use of the where clause in the class definition is not necessary but is used in this example to access the numeric operators & to show that the generic type is limited to the unsigned numbers class and does not affect the interfaces directly
/// </summary>
/// <typeparam name="T"></typeparam>
public class UnSignedNumbersOnly<[AllowedTypes<byte, ushort, uint, ulong, nuint>] T> where T : struct, IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>, IComparisonOperators<T, T, bool>
{
    public T Number1 { get; set; }
    public T Number2 { get; set; }
    
    
    
    public T SubtractToZeroOrHigher()
    {
        T subNumber = Number1 - Number2;
        if (subNumber <= default(T))
        {
            subNumber = default(T);
        }
        return subNumber;
    }
    

    public static void OldCheckStyle(T number1 , T number2)
    {
        bool valid1 = (number1 is uint or ulong or byte or ushort or nuint);
        bool valid2 = (number2 is uint or ulong or byte or ushort or nuint);
        if (valid1 && valid2)
        {
            UnSignedNumbersOnly<T> unSignedNumbersOnly = new();
            unSignedNumbersOnly.Add(number1, number2);
        }
    }
}

public static class UnSIgnedNumberExtensions
{
    public static T Add<[DeniedTypes<ulong>]T>(this UnSignedNumbersOnly<T> unsigned, T number1, T number2) where T : struct, IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>, IComparisonOperators<T, T, bool>
    {
        return number1 + number2;
    }
}


/// <summary>
/// The following class is an example of how to use the AllowedTypes attribute to limit the types that can be used in a generic class
/// It contains no specific differences from the UnSignedNumbersOnly class except that the generic type T can only be of type int or long
/// </summary>
/// <typeparam name="T"></typeparam>
public class IntsAndLongsOnly<[AllowedTypes<int,long>]T> where T : struct, IAdditionOperators<T, T, T>, ISubtractionOperators<T, T, T>, IComparisonOperators<T, T, bool>
{
    public T Add(T number1, T number2)
    {
        return number1 + number2;
    }
    
    public T Subtract(T number1, T number2)
    {
        T subNumber = number1 - number2;
        if (subNumber <= default(T))
        {
            subNumber = default(T);
        }
        return subNumber;
    }

    public static void OldCheckStyle(T number1 , T number2)
    {
        bool valid1 = (number1 is int or long);
        bool valid2 = (number2 is int or long);
        if (valid1 && valid2)
        {
            IntsAndLongsOnly<T> intsAndLongsOnly = new();
            intsAndLongsOnly.Add(number1, number2);
        }
    }
}

public class CustomNumberAddition
{
    public T Add<[DeniedTypes<long,ulong>]T>(T number1, T number2) where T : struct, IAdditionOperators<T, T, T>
    {
        return number1 + number2;
    }
    
    public static T StaticAdd<[DeniedTypes<long,ulong>]T>(T number1, T number2) where T : struct, IAdditionOperators<T, T, T>
    {
        return number1 + number2;
    }
}