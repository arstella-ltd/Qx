using Qx.Services;
using Qx.Tests.Helpers;

namespace Qx.Tests.Services;

public sealed class OpenAIServiceTests : TestBase
{
    [Fact]
    public void ConstructorWithNullApiKeyShouldThrow()
    {
        // Arrange & Act & Assert
        var action = () => new OpenAIService(null!);
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void ConstructorWithEmptyApiKeyShouldThrow()
    {
        // Arrange & Act & Assert
        var action = () => new OpenAIService(string.Empty);
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void ConstructorWithWhitespaceApiKeyShouldThrow()
    {
        // Arrange & Act & Assert
        var action = () => new OpenAIService("   ");
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void ConstructorWithValidApiKeyShouldSucceed()
    {
        // Arrange & Act
        var service = new OpenAIService("test-api-key");
        
        // Assert
        service.Should().NotBeNull();
        service.Should().BeAssignableTo<IOpenAIService>();
    }
}