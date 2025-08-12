using Qx.Tests.Helpers;

namespace Qx.Tests;

public class ProgramTests : TestBase
{
    [Fact]
    public void Main_WithNoArguments_ShouldReturnZero()
    {
        // Arrange
        string[] args = Array.Empty<string>();

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Main_WithHelpOption_ShouldReturnZero()
    {
        // Arrange
        string[] args = new[] { "--help" };

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Main_WithVersionCommand_ShouldReturnZero()
    {
        // Arrange
        string[] args = new[] { "version" };

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Main_WithQuery_NoApiKey_ShouldReturnAuthenticationError()
    {
        // Arrange
        string[] args = new[] { "test query" };
        SetEnvironmentVariable("OPENAI_API_KEY", null);

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(TestConstants.ExitCodes.AuthenticationError);
    }

    [Theory]
    [InlineData("--effort", "low")]
    [InlineData("--effort", "medium")]
    [InlineData("--effort", "high")]
    public void Main_WithValidEffortOption_ShouldAcceptValue(string option, string value)
    {
        // Arrange
        string[] args = new[] { "test query", option, value };
        SetEnvironmentVariable("OPENAI_API_KEY", TestConstants.TestApiKey);

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(0); // Currently returns 0 as OpenAI integration is not implemented
    }

    [Theory]
    [InlineData("--context", "low")]
    [InlineData("--context", "medium")]
    [InlineData("--context", "high")]
    public void Main_WithValidContextOption_ShouldAcceptValue(string option, string value)
    {
        // Arrange
        string[] args = new[] { "test query", option, value };
        SetEnvironmentVariable("OPENAI_API_KEY", TestConstants.TestApiKey);

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Main_WithTimeoutOption_ShouldAcceptIntValue()
    {
        // Arrange
        string[] args = new[] { "test query", "--timeout", "30" };
        SetEnvironmentVariable("OPENAI_API_KEY", TestConstants.TestApiKey);

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Main_WithNoSearchOption_ShouldAcceptFlag()
    {
        // Arrange
        string[] args = new[] { "test query", "--no-search" };
        SetEnvironmentVariable("OPENAI_API_KEY", TestConstants.TestApiKey);

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Main_WithMultipleOptions_ShouldAcceptAllValues()
    {
        // Arrange
        string[] args = new[] 
        { 
            "test query with multiple words",
            "--effort", "high",
            "--context", "low",
            "--timeout", "120",
            "--no-search"
        };
        SetEnvironmentVariable("OPENAI_API_KEY", TestConstants.TestApiKey);

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(0);
    }
}