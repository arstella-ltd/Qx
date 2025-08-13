using System.ComponentModel.DataAnnotations;

namespace Qx.Models;

/// <summary>
/// Represents the application configuration.
/// </summary>
internal sealed class Configuration
{
    /// <summary>
    /// Gets or sets the OpenAI API key.
    /// </summary>
    [Required(ErrorMessage = "API key is required")]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API options.
    /// </summary>
    public ApiOptions Options { get; set; } = new();

    /// <summary>
    /// Gets or sets the retry policy configuration.
    /// </summary>
    public RetryPolicy RetryPolicy { get; set; } = new();

    /// <summary>
    /// Gets or sets the base URL for the OpenAI API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets whether to enable debug mode.
    /// </summary>
    public bool DebugMode { get; set; }

    /// <summary>
    /// Gets or sets the organization ID for OpenAI API.
    /// </summary>
    public string? OrganizationId { get; set; }

    /// <summary>
    /// Validates the configuration.
    /// </summary>
    /// <returns>True if the configuration is valid; otherwise, false.</returns>
    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            return false;
        }

        if (!Options.Validate())
        {
            return false;
        }

        if (!RetryPolicy.Validate())
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Creates a configuration from environment variables.
    /// </summary>
    /// <returns>A new configuration instance.</returns>
    public static Configuration FromEnvironment()
    {
        Configuration config = new Configuration
        {
            ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty,
            BaseUrl = Environment.GetEnvironmentVariable("OPENAI_API_BASE_URL"),
            OrganizationId = Environment.GetEnvironmentVariable("OPENAI_ORGANIZATION_ID")
        };

        string? debugMode = Environment.GetEnvironmentVariable("QX_DEBUG");
        if (bool.TryParse(debugMode, out bool debug))
        {
            config.DebugMode = debug;
        }

        string? timeout = Environment.GetEnvironmentVariable("QX_TIMEOUT");
        if (int.TryParse(timeout, out int timeoutSeconds))
        {
            config.Options.DefaultTimeout = TimeSpan.FromSeconds(timeoutSeconds);
        }

        return config;
    }
}

/// <summary>
/// Represents retry policy configuration.
/// </summary>
internal sealed class RetryPolicy
{
    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    [Range(0, 10, ErrorMessage = "MaxAttempts must be between 0 and 10")]
    public int MaxAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the initial delay between retries.
    /// </summary>
    public TimeSpan InitialDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the backoff multiplier for exponential backoff.
    /// </summary>
    [Range(1.0, 5.0, ErrorMessage = "BackoffMultiplier must be between 1.0 and 5.0")]
    public double BackoffMultiplier { get; set; } = 2.0;

    /// <summary>
    /// Gets or sets the maximum delay between retries.
    /// </summary>
    public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets whether to retry on timeout.
    /// </summary>
    public bool RetryOnTimeout { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to retry on rate limit errors.
    /// </summary>
    public bool RetryOnRateLimit { get; set; } = true;

    /// <summary>
    /// Calculates the delay for a specific retry attempt.
    /// </summary>
    /// <param name="attempt">The retry attempt number (0-based).</param>
    /// <returns>The delay duration.</returns>
    public TimeSpan CalculateDelay(int attempt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(attempt);

        TimeSpan delay = TimeSpan.FromMilliseconds(
            InitialDelay.TotalMilliseconds * Math.Pow(BackoffMultiplier, attempt));

        return delay > MaxDelay ? MaxDelay : delay;
    }

    /// <summary>
    /// Validates the retry policy configuration.
    /// </summary>
    /// <returns>True if the configuration is valid; otherwise, false.</returns>
    public bool Validate()
    {
        if (MaxAttempts < 0 || MaxAttempts > 10)
        {
            return false;
        }

        if (InitialDelay <= TimeSpan.Zero)
        {
            return false;
        }

        if (BackoffMultiplier < 1.0 || BackoffMultiplier > 5.0)
        {
            return false;
        }

        if (MaxDelay <= TimeSpan.Zero)
        {
            return false;
        }

        return true;
    }
}
