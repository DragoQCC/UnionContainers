using DemoApp.Common;
using FluentAssertions;
using HelpfulTypesAndExtensions;

namespace UnionContainers.Tests.Unit;

public class UnionContainerT2PlusTests_GetResult
{
    
    [Test]
    public async Task MatchOnT1ResultContainer()
    {
        // Arrange
        var container = new UnionContainer<Programmer,Manager>(new Programmer("person",Guid.NewGuid(),DateTime.UtcNow));
        
        // Act
        Programmer result = container.Match(
            onT1Result: result => result,
            onT2Result: result => new Programmer("person1",Guid.NewGuid(),DateTime.UtcNow),
            onNoResult: () => new Programmer("person2",Guid.NewGuid(),DateTime.UtcNow));
        
        // Assert
        result.Should().BeOfType<Programmer>();
        result.Name.Should().Be("person");
    }
    
    
    [Test]
    public async Task MatchOnT2ResultContainer()
    {
        // Arrange
        UnionContainer<Programmer,Manager> container = new UnionContainer<Programmer,Manager>(new Manager("person",Guid.NewGuid(),DateTime.UtcNow));
        
        // Act
        Manager result = container.Match(
            onT1Result: result => new Manager("person1",Guid.NewGuid(),DateTime.UtcNow),
            onT2Result: result => result,
            onNoResult: () => new Manager("person2",Guid.NewGuid(),DateTime.UtcNow));
        
        // Assert
        result.Should().BeOfType<Manager>();
        result.Name.Should().Be("person");
    }
    
    [Test]
    public async Task MatchOnT2ResultContainerDefaultTest()
    {
        // Arrange
        bool isProgrammer = false;
        bool isManager = false;
        bool defaultResult = false;
        UnionContainer<Programmer,Manager> container = new UnionContainer<Programmer,Manager>(new Manager("person",Guid.NewGuid(),DateTime.UtcNow));
        
        // Act
        container.Match(
            onT1Result: (result) => isProgrammer = true,
            onT2Result: (result) => isManager = true,
            onNoResult: new Action(() => defaultResult = true));
        
        // Assert
        isProgrammer.Should().BeFalse();
        defaultResult.Should().BeFalse();
        isManager.Should().BeTrue();
    }

    [Test]
    public async Task MatchOnErrorContainer()
    {
        // Arrange
        string errorMessage = "";
        var errorContainer =  UnionContainerFactory.CreateErrorContainer<int>(ClientErrors.Forbidden("Foridden"));
        //act
        errorContainer.IfErrorDo(x => errorMessage = x.Message);
        //assert
        errorMessage.Should().Be("Foridden");
    }
}