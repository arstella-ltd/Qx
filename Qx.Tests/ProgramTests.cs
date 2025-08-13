using Qx.Tests.Helpers;

namespace Qx.Tests;

public sealed class ProgramTests : TestBase
{
    [Fact]
    public void MainWithNoArgumentsShouldReturnZero()
    {
        // Arrange
        string[] args = Array.Empty<string>();

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(0); // No query provided, returns 0
    }

    [Fact]
    public void MainWithHelpOptionShouldReturnZero()
    {
        // Arrange
        string[] args = new[] { "--help" };

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(0); // Help option should return 0
    }

    [Fact]
    public void MainWithVersionCommandShouldReturnZero()
    {
        // Arrange
        string[] args = new[] { "--version" };

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(0); // Version option should return 0
    }

    [Fact]
    public void MainWithQueryNoApiKeyShouldReturnAuthenticationError()
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
    public void MainWithValidEffortOptionShouldAcceptValue(string option, string value)
    {
        // Arrange
        string[] args = new[] { "test query", option, value };
        SetEnvironmentVariable("OPENAI_API_KEY", TestConstants.TestApiKey);

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(1); // Returns 1 as actual API call fails with test API key
    }

    [Theory]
    [InlineData("--context", "low")]
    [InlineData("--context", "medium")]
    [InlineData("--context", "high")]
    public void MainWithValidContextOptionShouldAcceptValue(string option, string value)
    {
        // Arrange
        string[] args = new[] { "test query", option, value };
        SetEnvironmentVariable("OPENAI_API_KEY", TestConstants.TestApiKey);

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(1); // Returns 1 as actual API call fails with test API key
    }

    [Fact]
    public void MainWithTimeoutOptionShouldAcceptIntValue()
    {
        // Arrange
        string[] args = new[] { "test query", "--timeout", "30" };
        SetEnvironmentVariable("OPENAI_API_KEY", TestConstants.TestApiKey);

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(1); // Returns 1 as actual API call fails with test API key
    }

    [Fact]
    public void MainWithNoSearchOptionShouldAcceptFlag()
    {
        // Arrange
        string[] args = new[] { "test query", "--no-search" };
        SetEnvironmentVariable("OPENAI_API_KEY", TestConstants.TestApiKey);

        // Act
        int result = Program.Main(args);

        // Assert
        result.Should().Be(1); // Returns 1 as actual API call fails with test API key
    }

    [Fact]
    public void MainWithMultipleOptionsShouldAcceptAllValues()
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
        result.Should().Be(1); // Returns 1 as actual API call fails with test API key
    }
}