using FluentAssertions;
using Qx.Models;
using Xunit;

namespace Qx.Tests.Models;

public class ResponseTests
{
    [Fact]
    public void Success_CreatesSuccessfulResponse()
    {
        // Arrange
        const string content = "Test response content";

        // Act
        var response = Response.Success(content);

        // Assert
        response.Content.Should().Be(content);
        response.IsComplete.Should().BeTrue();
        response.IsSuccess.Should().BeTrue();
        response.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Success_WithNullContent_CreatesResponseWithEmptyContent()
    {
        // Act
        var response = Response.Success(null!);

        // Assert
        response.Content.Should().BeEmpty();
        response.IsComplete.Should().BeTrue();
        response.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Failure_CreatesFailedResponse()
    {
        // Arrange
        const string errorMessage = "Test error message";

        // Act
        var response = Response.Failure(errorMessage);

        // Assert
        response.ErrorMessage.Should().Be(errorMessage);
        response.IsComplete.Should().BeTrue();
        response.IsSuccess.Should().BeFalse();
        response.Content.Should().BeEmpty();
    }

    [Fact]
    public void IsSuccess_WhenCompleteWithoutError_ReturnsTrue()
    {
        // Arrange
        var response = new Response
        {
            IsComplete = true,
            ErrorMessage = null
        };

        // Act & Assert
        response.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void IsSuccess_WhenIncomplete_ReturnsFalse()
    {
        // Arrange
        var response = new Response
        {
            IsComplete = false,
            ErrorMessage = null
        };

        // Act & Assert
        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void IsSuccess_WhenHasError_ReturnsFalse()
    {
        // Arrange
        var response = new Response
        {
            IsComplete = true,
            ErrorMessage = "Some error"
        };

        // Act & Assert
        response.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void DefaultConstructor_InitializesWithDefaults()
    {
        // Act
        var response = new Response();

        // Assert
        response.Content.Should().BeEmpty();
        response.IsComplete.Should().BeFalse();
        response.ErrorMessage.Should().BeNull();
        response.Metadata.Should().NotBeNull();
    }

    [Fact]
    public void ResponseMetadata_DefaultConstructor_InitializesWithDefaults()
    {
        // Act
        var metadata = new ResponseMetadata();

        // Assert
        metadata.Model.Should().BeEmpty();
        metadata.PromptTokens.Should().Be(0);
        metadata.CompletionTokens.Should().Be(0);
        metadata.TotalTokens.Should().Be(0);
        metadata.ResponseTime.Should().Be(TimeSpan.Zero);
        metadata.WebSearchUsed.Should().BeFalse();
        metadata.WebSearchResultsCount.Should().Be(0);
        metadata.FinishReason.Should().BeEmpty();
        metadata.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ResponseMetadata_CanSetAllProperties()
    {
        // Arrange
        var metadata = new ResponseMetadata();
        var now = DateTimeOffset.UtcNow;

        // Act
        metadata.Model = "gpt-4";
        metadata.PromptTokens = 100;
        metadata.CompletionTokens = 200;
        metadata.TotalTokens = 300;
        metadata.ResponseTime = TimeSpan.FromSeconds(5);
        metadata.WebSearchUsed = true;
        metadata.WebSearchResultsCount = 10;
        metadata.FinishReason = "stop";
        metadata.CreatedAt = now;

        // Assert
        metadata.Model.Should().Be("gpt-4");
        metadata.PromptTokens.Should().Be(100);
        metadata.CompletionTokens.Should().Be(200);
        metadata.TotalTokens.Should().Be(300);
        metadata.ResponseTime.Should().Be(TimeSpan.FromSeconds(5));
        metadata.WebSearchUsed.Should().BeTrue();
        metadata.WebSearchResultsCount.Should().Be(10);
        metadata.FinishReason.Should().Be("stop");
        metadata.CreatedAt.Should().Be(now);
    }
}
