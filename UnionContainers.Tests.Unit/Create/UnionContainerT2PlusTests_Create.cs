using DemoApp;
using DemoApp.Common;
using FluentAssertions;
using HelpfulTypesAndExtensions;

namespace UnionContainers.Tests.Unit;

public class UnionContainerT2PlusTests_Create
{
    [Test]
    public async Task CreateEmptyContainer()
    {
        //arrange
        UnionContainer<Programmer,Manager,HrPerson,ManagerInTraining,NewHire> container = new();
        //assert
        container.GetState().Should().Be(UnionContainerState.Empty);
    }
    
    [Test]
    public async Task CreateContainerWithResult()
    {
        //arrange
        UnionContainer<Programmer, Manager> container = Program.GetEmployeeOrManagerByNameOrId(Program.manager1.Name);
        //assert
        container.GetState().Should().Be(UnionContainerState.Result);
        container.Match(
            programmer => throw new Exception("Should not be a programmer"), 
            manager => manager.Name.Should().Be(Program.manager1.Name),
            () => throw new Exception("Should not be empty")
            );
    }

    [Test]
    public async Task CreateContainerWithError()
    {
        //arrange
        UnionContainer<Programmer,Manager,HrPerson,ManagerInTraining,NewHire> container = new(new ResourceErrors.NotFoundError());
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