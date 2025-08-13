using FluentAssertions;
using Qx.Models;
using Xunit;

namespace Qx.Tests.Models;

public class QueryTests
{
    [Fact]
    public void Constructor_WithContent_SetsContentProperty()
    {
        // Arrange
        const string content = "Test query content";

        // Act
        var query = new Query(content);

        // Assert
        query.Content.Should().Be(content);
    }

    [Fact]
    public void Constructor_WithNullContent_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new Query(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("content");
    }

    [Fact]
    public void DefaultConstructor_InitializesWithDefaults()
    {
        // Act
        var query = new Query();

        // Assert
        query.Content.Should().BeEmpty();
        query.ReasoningEffort.Should().Be(EffortLevel.Medium);
        query.SearchContext.Should().Be(ContextSize.Medium);
        query.Timeout.Should().Be(TimeSpan.FromSeconds(60));
        query.Model.Should().Be("gpt-5");
        query.Temperature.Should().Be(0.7);
        query.MaxTokens.Should().Be(1000);
        query.EnableWebSearch.Should().BeTrue();
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(2.0)]
    public void Temperature_ValidValues_SetsCorrectly(double temperature)
    {
        // Arrange
        var query = new Query();

        // Act
        query.Temperature = temperature;

        // Assert
        query.Temperature.Should().Be(temperature);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2048)]
    [InlineData(4096)]
    public void MaxTokens_ValidValues_SetsCorrectly(int maxTokens)
    {
        // Arrange
        var query = new Query();

        // Act
        query.MaxTokens = maxTokens;

        // Assert
        query.MaxTokens.Should().Be(maxTokens);
    }

    [Theory]
    [InlineData(EffortLevel.Low)]
    [InlineData(EffortLevel.Medium)]
    [InlineData(EffortLevel.High)]
    internal void ReasoningEffort_AllLevels_SetsCorrectly(EffortLevel effort)
    {
        // Arrange
        var query = new Query();

        // Act
        query.ReasoningEffort = effort;

        // Assert
        query.ReasoningEffort.Should().Be(effort);
    }

    [Theory]
    [InlineData(ContextSize.Low)]
    [InlineData(ContextSize.Medium)]
    [InlineData(ContextSize.High)]
    internal void SearchContext_AllSizes_SetsCorrectly(ContextSize context)
    {
        // Arrange
        var query = new Query();

        // Act
        query.SearchContext = context;

        // Assert
        query.SearchContext.Should().Be(context);
    }

    [Fact]
    public void Timeout_CustomValue_SetsCorrectly()
    {
        // Arrange
        var query = new Query();
        var timeout = TimeSpan.FromSeconds(120);

        // Act
        query.Timeout = timeout;

        // Assert
        query.Timeout.Should().Be(timeout);
    }
}
