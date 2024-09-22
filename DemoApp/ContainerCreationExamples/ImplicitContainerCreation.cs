/*using System;
using System.Net;
using DemoApp.Common;
using UnionContainers.Containers.Standard;
using static DemoApp.Program;

namespace DemoApp.ContainerCreationExamples;

public class ImplicitContainerCreation
{
    public static void ImplicitContainerCreationExample()
    {
        // implicit conversion
        Console.WriteLine("Implicit container creation example from method result");
        UnionContainer<Employee, Manager> container_two = TryGetEmployeeByName(targetNameTwo)!;
    }
    
    public static void ImplicitContainerCreationExampleTwo()
    {
        // implicit conversion
        Console.WriteLine("Implicit container creation example from object conversion");
        UnionContainer<string, int, HttpStatusCode> container = HttpStatusCode.OK;
    }
}*/