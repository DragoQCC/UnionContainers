using DemoApp;
using DemoApp.Common;

namespace UnionContainers.Tests.Unit.ExtensionTests;

public class ContainerExtension_Tests
{
    [Test]
    public async Task ForState_Test()
    {
        UnionContainer<int> container = RandomExampleMethods.Divide(1, 0);
        container.ForState(
            null,
            (UnionContainerState.Result, () =>Console.WriteLine("The container has a result")),
            (UnionContainerState.Error, () => Console.WriteLine("The container has an error")),
            (UnionContainerState.Exception, () => Console.WriteLine("The container has an exception")),
            (UnionContainerState.Empty, () => Console.WriteLine("The container has no result"))
            );
    }
}