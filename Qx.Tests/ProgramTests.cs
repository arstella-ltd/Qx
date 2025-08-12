namespace Qx.Tests;

public class ProgramTests
{
    [Fact]
    public async Task Main_WithNoArguments_ShouldReturnZero()
    {
        // Arrange
        var args = new string[] { };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task Main_WithHelpOption_ShouldReturnZero()
    {
        // Arrange
        var args = new[] { "--help" };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task Main_WithVersionCommand_ShouldReturnZero()
    {
        // Arrange
        var args = new[] { "version" };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task Main_WithQuery_NoApiKey_ShouldReturnAuthenticationError()
    {
        // Arrange
        var args = new[] { "test query" };
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);

        // Act
        var result = await Program.Main(args);

        // Assert
        result.Should().Be(3); // Authentication error code
    }

    [Theory]
    [InlineData("--effort", "low")]
    [InlineData("--effort", "medium")]
    [InlineData("--effort", "high")]
    public async Task Main_WithValidEffortOption_ShouldAcceptValue(string option, string value)
    {
        // Arrange
        var args = new[] { "test query", option, value };
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-key");

        // Act
        var result = await Program.Main(args);

        // Assert
        result.Should().Be(0); // Currently returns 0 as OpenAI integration is not implemented
    }

    [Theory]
    [InlineData("--context", "low")]
    [InlineData("--context", "medium")]
    [InlineData("--context", "high")]
    public async Task Main_WithValidContextOption_ShouldAcceptValue(string option, string value)
    {
        // Arrange
        var args = new[] { "test query", option, value };
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-key");

        // Act
        var result = await Program.Main(args);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task Main_WithTimeoutOption_ShouldAcceptIntValue()
    {
        // Arrange
        var args = new[] { "test query", "--timeout", "30" };
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-key");

        // Act
        var result = await Program.Main(args);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task Main_WithNoSearchOption_ShouldAcceptFlag()
    {
        // Arrange
        var args = new[] { "test query", "--no-search" };
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-key");

        // Act
        var result = await Program.Main(args);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task Main_WithMultipleOptions_ShouldAcceptAllValues()
    {
        // Arrange
        var args = new[] 
        { 
            "test query with multiple words",
            "--effort", "high",
            "--context", "low",
            "--timeout", "120",
            "--no-search"
        };
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-key");

        // Act
        var result = await Program.Main(args);

        // Assert
        result.Should().Be(0);
    }
}