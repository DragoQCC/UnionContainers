using DemoApp;
using DemoApp.Common;
using FluentAssertions;

namespace UnionContainers.Tests.Unit.FromMethods;

public class UnionContainers_FromUserMethods
{
    [Test,Arguments("Bob Stevens")]
    public async Task ContainerFromMethodExample(string targetManagerName)
    {
        //arrange
        UnionContainer<Programmer, Manager> container = DemoApp.Program.GetEmployeeOrManagerByNameOrId(targetManagerName);
        
        //assert
        container.GetState().Should().Be(UnionContainerState.Result);
        container.Match(
            result => throw new Exception("Should not be a Programmer"), 
            result =>
            {
                result.Name.Should().Be(targetManagerName);
                result.Should().BeOfType<Manager>();
            },
            () => throw new Exception("Should not be an error")
        );
    }

    [Test]
    public async Task ContainerFromMethodExceptionWraping()
    {
        var container = await UnionContainer<HttpResponseMessage>.MethodToContainer(async () => await Program.TryConnectAsync("localhost", "http", 5005));
        
        container.GetState().Should().Be(UnionContainerState.Exception);
    }
}