using System.Net;

namespace DemoApp;

public class FunctionalDemo
{
    public async Task Run()
    {
        Console.WriteLine("------------------Functional Extension Methods Demo-----------------------------");
        
        //TryCatchWrapper example
        Console.WriteLine("------------------TryCatchWrapper Example---------------------------------------");
        await FunctionalExtensionsExamples.FunctionalExtensionsDemo.TryCatchWrapper();
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("------------------CheckIfExample Example----------------------------------------");
        try
        {
            FunctionalExtensionsExamples.FunctionalExtensionsDemo.CheckIfExample();
            FunctionalExtensionsExamples.FunctionalExtensionsDemo.CheckIfExample2();
            await FunctionalExtensionsExamples.FunctionalExtensionsDemo.CheckIfExample3(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("------------------ForEachExample Example----------------------------------------");
        try
        {
            FunctionalExtensionsExamples.FunctionalExtensionsDemo.ForEachExample();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("------------------ContinueWithExample Example-----------------------------------");
        try
        {
            FunctionalExtensionsExamples.FunctionalExtensionsDemo.ContinueWithExample();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("------------------ThrowIfNullExample with null Example--------------------------");
        try
        {
            FunctionalExtensionsExamples.FunctionalExtensionsDemo.ThrowIfNullExample(null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("------------------ThrowIfNullExample without null Example-----------------------");
        try
        {
            FunctionalExtensionsExamples.FunctionalExtensionsDemo.ThrowIfNullExample("Steven");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
    }
}