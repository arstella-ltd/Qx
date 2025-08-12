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
    
    [Fact]
    public async Task QueryAsyncWithNullQueryShouldThrow()
    {
        // Arrange
        var service = new OpenAIService("test-api-key");
        var options = new QueryOptions();
        
        // Act & Assert
        await service.Invoking(s => s.QueryAsync(null!, options))
            .Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task QueryAsyncWithEmptyQueryShouldThrow()
    {
        // Arrange
        var service = new OpenAIService("test-api-key");
        var options = new QueryOptions();
        
        // Act & Assert
        await service.Invoking(s => s.QueryAsync(string.Empty, options))
            .Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task QueryAsyncWithNullOptionsShouldThrow()
    {
        // Arrange
        var service = new OpenAIService("test-api-key");
        
        // Act & Assert
        await service.Invoking(s => s.QueryAsync("test query", null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }
    
    [Theory]
    [InlineData("low", "low")]
    [InlineData("medium", "medium")]
    [InlineData("high", "high")]
    public void QueryOptionsEffortLevelShouldBeConfigurable(string effortLevel, string expected)
    {
        // Arrange & Act
        var options = new QueryOptions { EffortLevel = effortLevel };
        
        // Assert
        options.EffortLevel.Should().Be(expected);
    }
    
    [Theory]
    [InlineData("low", "low")]
    [InlineData("medium", "medium")]
    [InlineData("high", "high")]
    public void QueryOptionsContextSizeShouldBeConfigurable(string contextSize, string expected)
    {
        // Arrange & Act
        var options = new QueryOptions { ContextSize = contextSize };
        
        // Assert
        options.ContextSize.Should().Be(expected);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void QueryOptionsEnableWebSearchShouldBeConfigurable(bool enableWebSearch)
    {
        // Arrange & Act
        var options = new QueryOptions { EnableWebSearch = enableWebSearch };
        
        // Assert
        options.EnableWebSearch.Should().Be(enableWebSearch);
    }
    
    [Theory]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(120)]
    public void QueryOptionsTimeoutSecondsShouldBeConfigurable(int timeout)
    {
        // Arrange & Act
        var options = new QueryOptions { TimeoutSeconds = timeout };
        
        // Assert
        options.TimeoutSeconds.Should().Be(timeout);
    }
    
    [Theory]
    [InlineData("gpt-4")]
    [InlineData("gpt-4o")]
    [InlineData("gpt-4o-mini")]
    [InlineData("gpt-3.5-turbo")]
    public void QueryOptionsModelShouldBeConfigurable(string model)
    {
        // Arrange & Act
        var options = new QueryOptions { Model = model };
        
        // Assert
        options.Model.Should().Be(model);
    }
    
    [Theory]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(1000)]
    [InlineData(null)]
    public void QueryOptionsMaxTokensShouldBeConfigurable(int? maxTokens)
    {
        // Arrange & Act
        var options = new QueryOptions { MaxTokens = maxTokens };
        
        // Assert
        options.MaxTokens.Should().Be(maxTokens);
    }
    
    [Theory]
    [InlineData(0.0)]
    [InlineData(0.7)]
    [InlineData(1.0)]
    [InlineData(2.0)]
    public void QueryOptionsTemperatureShouldBeConfigurable(double temperature)
    {
        // Arrange & Act
        var options = new QueryOptions { Temperature = temperature };
        
        // Assert
        options.Temperature.Should().Be(temperature);
    }
    
    [Fact]
    public void QueryOptionsDefaultValuesShouldBeCorrect()
    {
        // Arrange & Act
        var options = new QueryOptions();
        
        // Assert
        options.EffortLevel.Should().Be("medium");
        options.ContextSize.Should().Be("medium");
        options.EnableWebSearch.Should().BeTrue();
        options.TimeoutSeconds.Should().Be(60);
        options.Model.Should().Be("gpt-4o-mini");
        options.MaxTokens.Should().BeNull();
        options.Temperature.Should().Be(0.7);
        options.SystemPrompt.Should().BeNull();
    }
    
    [Fact]
    public void QueryOptionsWithCustomSystemPromptShouldBeConfigurable()
    {
        // Arrange
        const string customPrompt = "You are a helpful coding assistant.";
        
        // Act
        var options = new QueryOptions { SystemPrompt = customPrompt };
        
        // Assert
        options.SystemPrompt.Should().Be(customPrompt);
    }
}