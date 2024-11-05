using System.Net;
using DemoApp.Common;
using FluentAssertions;
using HelpfulTypesAndExtensions;

namespace UnionContainers.Tests.Unit;

public class UnionContainerTests_ExplicitCasts
{
    [Test]
    public async Task ExplicitCastToUnionContainer()
    {
        //arrange
        Programmer steve = new("Steve", Guid.NewGuid(), DateTime.Now);
        
        //act
        UnionContainer<Programmer> container = steve.ToContainer();
       
        //assert
        container.GetState().Should().Be(UnionContainerState.Result);
        container.Match(
            result => result.Should().Be(steve),
            () => throw new Exception("Should not be an error")
        );
    }
}