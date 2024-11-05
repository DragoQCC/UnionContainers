using DemoApp.Common;
using FluentAssertions;
using HelpfulTypesAndExtensions;

namespace UnionContainers.Tests.Unit;

public class UnionContainerT1Tests_GetResult
{
    
    [Test]
    public async Task GetEmptyResultFromContainer()
    {
        // Arrange
        bool updated = false;
        var container = new UnionContainer<int>();
        
        // Act
        container.IfEmptyDo(() => updated = true);
        
        // Assert
        updated.Should().BeTrue();
    }
    
    [Test]
    public async Task MatchOnResultContainer()
    {
        // Arrange
        var container = new UnionContainer<NewHire>(new NewHire());
        
        // Act
        NewHire result = container.Match(
            onResult: result => result,
            onNoResult: () => new NewHire());
        
        // Assert
        result.Should().BeOfType<NewHire>();
        result.Name.Should().Be("New Hire");
    }
    
    [Test]
    public async Task MatchOnNoResultContainer()
    {
        // Arrange
        var container = new UnionContainer<NewHire>(new NewHire());
        
        // Act
        NewHire result = container.Match(
            onResult: result => result,
            onNoResult: () => new NewHire());
        
        // Assert
        result.Should().BeOfType<NewHire>();
        result.Name.Should().Be("New Hire");
    }

    [Test]
    public async Task GetErrorFromContainer()
    {
        // Arrange
        var container = new UnionContainer<NewHire>(new NewHire());
    
        // Act
        container.AddError(ClientErrors.ValidationFailure("Validation Failed"));
        IError error = container.GetErrors().First();
    
        // Assert
        error.Should().BeOfType<ClientErrors.ValidationFailureError>();
        error.Message.Should().Be("Validation Failed");
    }
    
    [Test]
    public async Task GetMultipleErrorsFromContainer()
    {
        // Arrange
        var container = new UnionContainer<NewHire>(new NewHire());
    
        // Act
        container.AddError(ClientErrors.ValidationFailure("Validation Failed"));
        container.AddError(ResourceErrors.NotFound("Resource Not Found"));
        List<IError> errors = container.GetErrors();
    
        // Assert
        errors.Should().HaveCount(2);
        errors[0].Should().BeOfType<ClientErrors.ValidationFailureError>();
        errors[0].Name.Should().Be("Validation Failure");
        errors[1].Should().BeOfType<ResourceErrors.NotFoundError>();
        errors[1].Name.Should().Be("Not Found");
    }

    [Test]
    public async Task GetExceptionFromContainer()
    {
        // Arrange
        var container = new UnionContainer<NewHire>(new NewHire());
    
        // Act
        container.AddError(new Exception("An exception occurred"));
        IError error = container.GetErrors().First();
    
        // Assert
        error.Should().BeOfType<CustomErrors.ExceptionWrapperError>();
        container.GetState().Should().Be(UnionContainerState.Exception);
        error.Message.Should().Be("An exception occurred");
    }
}