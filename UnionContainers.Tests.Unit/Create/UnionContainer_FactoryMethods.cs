using FluentAssertions;
using HelpfulTypesAndExtensions;

namespace UnionContainers.Tests.Unit;

public class UnionContainer_FactoryMethods
{
    [Test]
    public async Task CreateEmptyContainer_FromFactory()
    {
        // Arrange
        // Act
        UnionContainer<int> container = UnionContainerFactory.CreateEmptyContainer<int>();
        
        // Assert
        container.GetState().Should().Be(UnionContainerState.Empty);
    }
    
    [Test]
    public async Task CreateErrorContainer_FromFactory()
    {
        // Arrange
        IError error = ServerErrors.ServiceUnavailable("Service is down");
        
        // Act
        UnionContainer<int> container = UnionContainerFactory.CreateErrorContainer<int>(error);
        
        // Assert
        container.GetState().Should().Be(UnionContainerState.Error);
        container.GetErrors().Should().Contain(error);
    }
    
    [Test]
    public async Task CreateExceptionContainer_FromFactory()
    {
        // Arrange
        Exception exception = new Exception();
        
        // Act
        UnionContainer<int> container = UnionContainerFactory.CreateExceptionContainer<int>(exception);
        
        // Assert
        container.GetState().Should().Be(UnionContainerState.Exception);
        container.GetErrors().Should().ContainSingle().Which.Should().BeOfType<CustomErrors.ExceptionWrapperError>();
    }
    
    [Test]
    public async Task CreateResultContainer_FromFactory()
    {
        // Arrange
        int number = 1;
        
        // Act
        UnionContainer<int> container = UnionContainerFactory.CreateResultContainer(number);
        
        // Assert
        container.GetState().Should().Be(UnionContainerState.Result);
        container.Match(result => result.Should().BeGreaterThanOrEqualTo(number), () => throw new Exception("Should not be empty"));
    }
    
}