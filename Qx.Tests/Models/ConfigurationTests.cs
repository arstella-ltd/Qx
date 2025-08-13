using FluentAssertions;
using Qx.Models;
using Xunit;

namespace Qx.Tests.Models;

public class ConfigurationTests
{
    [Fact]
    public void Validate_WithValidConfiguration_ReturnsTrue()
    {
        // Arrange
        var config = new Configuration
        {
            ApiKey = "test-api-key",
            Options = new ApiOptions(),
            RetryPolicy = new RetryPolicy()
        };

        // Act
        var result = config.Validate();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyApiKey_ReturnsFalse()
    {
        // Arrange
        var config = new Configuration
        {
            ApiKey = string.Empty,
            Options = new ApiOptions(),
            RetryPolicy = new RetryPolicy()
        };

        // Act
        var result = config.Validate();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithInvalidOptions_ReturnsFalse()
    {
        // Arrange
        var config = new Configuration
        {
            ApiKey = "test-api-key",
            Options = new ApiOptions { DefaultTemperature = -1 }, // Invalid temperature
            RetryPolicy = new RetryPolicy()
        };

        // Act
        var result = config.Validate();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void FromEnvironment_WithEnvironmentVariables_LoadsConfiguration()
    {
        // Arrange
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-key");
        Environment.SetEnvironmentVariable("OPENAI_API_BASE_URL", "https://api.test.com");
        Environment.SetEnvironmentVariable("OPENAI_ORGANIZATION_ID", "org-123");
        Environment.SetEnvironmentVariable("QX_DEBUG", "true");
        Environment.SetEnvironmentVariable("QX_TIMEOUT", "120");

        try
        {
            // Act
            var config = Configuration.FromEnvironment();

            // Assert
            config.ApiKey.Should().Be("test-key");
            config.BaseUrl.Should().Be("https://api.test.com");
            config.OrganizationId.Should().Be("org-123");
            config.DebugMode.Should().BeTrue();
            config.Options.DefaultTimeout.Should().Be(TimeSpan.FromSeconds(120));
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);
            Environment.SetEnvironmentVariable("OPENAI_API_BASE_URL", null);
            Environment.SetEnvironmentVariable("OPENAI_ORGANIZATION_ID", null);
            Environment.SetEnvironmentVariable("QX_DEBUG", null);
            Environment.SetEnvironmentVariable("QX_TIMEOUT", null);
        }
    }

    [Fact]
    public void FromEnvironment_WithoutEnvironmentVariables_ReturnsDefaults()
    {
        // Arrange - ensure no env vars are set
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);
        Environment.SetEnvironmentVariable("QX_DEBUG", null);
        Environment.SetEnvironmentVariable("QX_TIMEOUT", null);

        // Act
        var config = Configuration.FromEnvironment();

        // Assert
        config.ApiKey.Should().BeEmpty();
        config.BaseUrl.Should().BeNull();
        config.OrganizationId.Should().BeNull();
        config.DebugMode.Should().BeFalse();
        config.Options.DefaultTimeout.Should().Be(TimeSpan.FromSeconds(60));
    }
}

public class RetryPolicyTests
{
    [Fact]
    public void Validate_WithValidPolicy_ReturnsTrue()
    {
        // Arrange
        var policy = new RetryPolicy();

        // Act
        var result = policy.Validate();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void Validate_WithInvalidMaxAttempts_ReturnsFalse(int maxAttempts)
    {
        // Arrange
        var policy = new RetryPolicy { MaxAttempts = maxAttempts };

        // Act
        var result = policy.Validate();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithZeroInitialDelay_ReturnsFalse()
    {
        // Arrange
        var policy = new RetryPolicy { InitialDelay = TimeSpan.Zero };

        // Act
        var result = policy.Validate();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(6.0)]
    public void Validate_WithInvalidBackoffMultiplier_ReturnsFalse(double multiplier)
    {
        // Arrange
        var policy = new RetryPolicy { BackoffMultiplier = multiplier };

        // Act
        var result = policy.Validate();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(0, 1000)]
    [InlineData(1, 2000)]
    [InlineData(2, 4000)]
    [InlineData(3, 8000)]
    public void CalculateDelay_WithExponentialBackoff_ReturnsCorrectDelay(int attempt, int expectedMs)
    {
        // Arrange
        var policy = new RetryPolicy
        {
            InitialDelay = TimeSpan.FromMilliseconds(1000),
            BackoffMultiplier = 2.0,
            MaxDelay = TimeSpan.FromSeconds(30)
        };

        // Act
        var delay = policy.CalculateDelay(attempt);

        // Assert
        delay.TotalMilliseconds.Should().Be(expectedMs);
    }

    [Fact]
    public void CalculateDelay_ExceedsMaxDelay_ReturnsMaxDelay()
    {
        // Arrange
        var policy = new RetryPolicy
        {
            InitialDelay = TimeSpan.FromSeconds(10),
            BackoffMultiplier = 2.0,
            MaxDelay = TimeSpan.FromSeconds(30)
        };

        // Act
        var delay = policy.CalculateDelay(5); // Would be 320 seconds without max

        // Assert
        delay.Should().Be(TimeSpan.FromSeconds(30));
    }

    [Fact]
    public void CalculateDelay_WithNegativeAttempt_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var policy = new RetryPolicy();

        // Act
        var act = () => policy.CalculateDelay(-1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("attempt");
    }

    [Fact]
    public void DefaultConstructor_InitializesWithExpectedDefaults()
    {
        // Act
        var policy = new RetryPolicy();

        // Assert
        policy.MaxAttempts.Should().Be(3);
        policy.InitialDelay.Should().Be(TimeSpan.FromSeconds(1));
        policy.BackoffMultiplier.Should().Be(2.0);
        policy.MaxDelay.Should().Be(TimeSpan.FromSeconds(30));
        policy.RetryOnTimeout.Should().BeTrue();
        policy.RetryOnRateLimit.Should().BeTrue();
    }
}
