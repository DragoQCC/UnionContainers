using FluentAssertions;
using HelpfulTypesAndExtensions;

namespace UnionContainers.Tests.Unit;

public class UnionContainerT1Tests_Create
{
    [Test]
    public async Task CreateEmptyContainer()
    {
        //arrange
        UnionContainer<string> container = new();
        //assert
        container.GetState().Should().Be(UnionContainerState.Empty);
    }
    
    [Test]
    public async Task CreateContainerWithResult()
    {
        //arrange
        UnionContainer<string> container = new("result");
        //assert
        container.GetState().Should().Be(UnionContainerState.Result);
    }

    [Test]
    public async Task CreateContainerWithError()
    {
        //arrange
        UnionContainer<string> container = new(new ResourceErrors.NotFoundError());
        //assert
        container.GetState().Should().Be(UnionContainerState.Error);
    }
    
    [Test]
    public async Task CreateContainerWithMultipleErrors()
    {
        //arrange
        UnionContainer<string> container = new();
        //act
        container.AddError(new ResourceErrors.NotFoundError());
        container.AddError(new ClientErrors.ValidationFailureError());
        //assert
        container.GetState().Should().Be(UnionContainerState.Error);
    }

    [Test]
    public async Task CreateContainerWithException()
    {
        //arrange
        UnionContainer<string> container = new(new Exception("An exception occurred"));
        IError error = container.GetErrors().First();
        //assert
        container.GetState().Should().Be(UnionContainerState.Exception);
        error.Should().BeOfType<CustomErrors.ExceptionWrapperError>();
    }
    
    
}