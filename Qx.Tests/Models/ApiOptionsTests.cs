using FluentAssertions;
using Qx.Models;
using Xunit;

namespace Qx.Tests.Models;

public class ApiOptionsTests
{
    private static readonly string[] ExpectedAllowedModels = { "gpt-5", "gpt-5-nano", "gpt-5-mini", "gpt-4o", "gpt-4o-mini", "o3", "gpt-4.1", "gpt-4.1-mini", "gpt-4.1-nano" };
    [Fact]
    public void Validate_WithValidOptions_ReturnsTrue()
    {
        // Arrange
        var options = new ApiOptions();

        // Act
        var result = options.Validate();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyModel_ReturnsFalse()
    {
        // Arrange
        var options = new ApiOptions { DefaultModel = string.Empty };

        // Act
        var result = options.Validate();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(2.1)]
    public void Validate_WithInvalidTemperature_ReturnsFalse(double temperature)
    {
        // Arrange
        var options = new ApiOptions { DefaultTemperature = temperature };

        // Act
        var result = options.Validate();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4097)]
    public void Validate_WithInvalidMaxTokens_ReturnsFalse(int maxTokens)
    {
        // Arrange
        var options = new ApiOptions { DefaultMaxTokens = maxTokens };

        // Act
        var result = options.Validate();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithZeroTimeout_ReturnsFalse()
    {
        // Arrange
        var options = new ApiOptions { DefaultTimeout = TimeSpan.Zero };

        // Act
        var result = options.Validate();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1001)]
    public void Validate_WithInvalidRateLimit_ReturnsFalse(int rateLimit)
    {
        // Arrange
        var options = new ApiOptions { RateLimitPerMinute = rateLimit };

        // Act
        var result = options.Validate();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(999)]
    [InlineData(1000001)]
    public void Validate_WithInvalidTokenLimit_ReturnsFalse(int tokenLimit)
    {
        // Arrange
        var options = new ApiOptions { TokenLimitPerMinute = tokenLimit };

        // Act
        var result = options.Validate();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithModelNotInAllowedList_ReturnsFalse()
    {
        // Arrange
        var options = new ApiOptions
        {
            DefaultModel = "invalid-model",
            AllowedModels = new HashSet<string> { "gpt-3.5-turbo", "gpt-4" }
        };

        // Act
        var result = options.Validate();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("gpt-5", true)]
    [InlineData("gpt-5-nano", true)]
    [InlineData("gpt-4o", true)]
    [InlineData("o3", true)]
    [InlineData("invalid-model", false)]
    [InlineData("", false)]
    public void IsModelAllowed_WithVariousModels_ReturnsExpectedResult(string model, bool expected)
    {
        // Arrange
        var options = new ApiOptions();

        // Act
        var result = options.IsModelAllowed(model);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsModelAllowed_WithNullModel_ReturnsFalse()
    {
        // Arrange
        var options = new ApiOptions();

        // Act
        var result = options.IsModelAllowed(null!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void MergeWithCommandLine_WithAllParameters_OverridesDefaults()
    {
        // Arrange
        var options = new ApiOptions
        {
            DefaultModel = "gpt-3.5-turbo",
            DefaultTemperature = 0.7,
            DefaultMaxTokens = 1000
        };

        // Act
        var merged = options.MergeWithCommandLine("gpt-4", 1.5, 2000);

        // Assert
        merged.DefaultModel.Should().Be("gpt-4");
        merged.DefaultTemperature.Should().Be(1.5);
        merged.DefaultMaxTokens.Should().Be(2000);
    }

    [Fact]
    public void MergeWithCommandLine_WithNullParameters_KeepsDefaults()
    {
        // Arrange
        var options = new ApiOptions
        {
            DefaultModel = "gpt-3.5-turbo",
            DefaultTemperature = 0.7,
            DefaultMaxTokens = 1000
        };

        // Act
        var merged = options.MergeWithCommandLine(null, null, null);

        // Assert
        merged.DefaultModel.Should().Be("gpt-3.5-turbo");
        merged.DefaultTemperature.Should().Be(0.7);
        merged.DefaultMaxTokens.Should().Be(1000);
    }

    [Fact]
    public void MergeWithCommandLine_WithEmptyModel_KeepsDefault()
    {
        // Arrange
        var options = new ApiOptions { DefaultModel = "gpt-3.5-turbo" };

        // Act
        var merged = options.MergeWithCommandLine("", 1.0, 500);

        // Assert
        merged.DefaultModel.Should().Be("gpt-3.5-turbo");
        merged.DefaultTemperature.Should().Be(1.0);
        merged.DefaultMaxTokens.Should().Be(500);
    }

    [Fact]
    public void MergeWithCommandLine_PreservesOtherProperties()
    {
        // Arrange
        var options = new ApiOptions
        {
            DefaultEffort = EffortLevel.High,
            DefaultContext = ContextSize.Low,
            EnableWebSearchByDefault = false,
            SystemPrompt = "Test prompt",
            CustomHeaders = new Dictionary<string, string> { { "X-Test", "Value" } }
        };

        // Act
        var merged = options.MergeWithCommandLine("gpt-4", null, null);

        // Assert
        merged.DefaultEffort.Should().Be(EffortLevel.High);
        merged.DefaultContext.Should().Be(ContextSize.Low);
        merged.EnableWebSearchByDefault.Should().BeFalse();
        merged.SystemPrompt.Should().Be("Test prompt");
        merged.CustomHeaders.Should().ContainKey("X-Test");
    }

    [Fact]
    public void DefaultConstructor_InitializesWithExpectedDefaults()
    {
        // Act
        var options = new ApiOptions();

        // Assert
        options.DefaultEffort.Should().Be(EffortLevel.Medium);
        options.DefaultContext.Should().Be(ContextSize.Medium);
        options.DefaultTimeout.Should().Be(TimeSpan.FromSeconds(60));
        options.DefaultModel.Should().Be("gpt-5-nano");
        options.DefaultTemperature.Should().Be(1.0);
        options.DefaultMaxTokens.Should().Be(1000);
        options.EnableWebSearchByDefault.Should().BeTrue();
        options.StreamResponsesByDefault.Should().BeTrue();
        options.SystemPrompt.Should().BeNull();
        options.AllowedModels.Should().Contain(ExpectedAllowedModels);
        options.CustomHeaders.Should().BeEmpty();
        options.RateLimitPerMinute.Should().Be(60);
        options.TokenLimitPerMinute.Should().Be(90000);
    }
}
