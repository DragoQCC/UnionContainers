using System.Reflection;

namespace DemoApp.Common;

public static class MethodTimeLogger
{
    private static List<string> _loggedMethods = new List<string>();

    public static void Log(MethodBase methodBase, TimeSpan timeSpan, string message) // timespan can be a long as well
    {
        string duration = timeSpan.TotalMilliseconds > 1000 ? $"{timeSpan.TotalSeconds} seconds" : $"{timeSpan.TotalMilliseconds} ms";
        _loggedMethods.Add($"{methodBase.DeclaringType!.Name}.{methodBase.Name} - {message} completed in {duration}");
    }

    public static void PrintLoggedMethodResults()
    {
        Console.WriteLine();
        Console.WriteLine("------------------Logged Method Performance------------------------------------");
        foreach (var loggedMethod in _loggedMethods)
        {
            Console.WriteLine(loggedMethod);
        }
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
    }
}