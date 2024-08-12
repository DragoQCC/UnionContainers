using DemoApp.ContainerCreationExamples;
using DemoApp.TypeLimitingAttributeExamples;

namespace DemoApp;

public class AttributeDemo
{
    public async Task Run()
    {
        Console.WriteLine("------------------Custom Attributes Demo----------------------------------------");
        
        Console.WriteLine("------------------Allowed Types Examples------------------------------------");
        try
        {
            AllowedTypesUsageExamples.AllowedTypesUsageExample();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("------------------Denied Types Examples-------------------------------------");
        try
        {
            DeniedTypesExamples.DeniedTypesUsageExample();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("------------------Attribute Usage Examples----------------------------------");
        try
        {
            GenericClassUsageExamples.GenericUsageExample();
            await ExplicitContainerCreation.AllowedTypesFuncUseExample();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
    }
}