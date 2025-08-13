using System.ComponentModel.DataAnnotations;

namespace Qx.Models;

/// <summary>
/// Represents API-specific options for OpenAI integration.
/// </summary>
internal sealed class ApiOptions
{
    /// <summary>
    /// Gets or sets the default effort level for reasoning.
    /// </summary>
    public EffortLevel DefaultEffort { get; set; } = EffortLevel.Medium;

    /// <summary>
    /// Gets or sets the default context size for searches.
    /// </summary>
    public ContextSize DefaultContext { get; set; } = ContextSize.Medium;

    /// <summary>
    /// Gets or sets the default timeout for API calls.
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Gets or sets the default model to use.
    /// </summary>
    public string DefaultModel { get; set; } = "gpt-5";

    /// <summary>
    /// Gets or sets the default temperature for generation.
    /// </summary>
    [Range(0.0, 2.0, ErrorMessage = "DefaultTemperature must be between 0.0 and 2.0")]
    public double DefaultTemperature { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets the default maximum tokens.
    /// </summary>
    [Range(1, 4096, ErrorMessage = "DefaultMaxTokens must be between 1 and 4096")]
    public int DefaultMaxTokens { get; set; } = 1000;

    /// <summary>
    /// Gets or sets whether web search is enabled by default.
    /// </summary>
    public bool EnableWebSearchByDefault { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to stream responses by default.
    /// </summary>
    public bool StreamResponsesByDefault { get; set; } = true;

    /// <summary>
    /// Gets or sets the system prompt to prepend to queries.
    /// </summary>
    public string? SystemPrompt { get; set; }

    /// <summary>
    /// Gets the list of allowed models.
    /// </summary>
    public HashSet<string> AllowedModels { get; init; } = new()
    {
        "gpt-5",
        "gpt-4o",
        "gpt-4o-mini",
        "gpt-4-turbo",
        "gpt-4",
        "gpt-3.5-turbo"
    };

    /// <summary>
    /// Gets custom headers to include in API requests.
    /// </summary>
    public Dictionary<string, string> CustomHeaders { get; init; } = new();

    /// <summary>
    /// Gets or sets the rate limit per minute.
    /// </summary>
    [Range(1, 1000, ErrorMessage = "RateLimitPerMinute must be between 1 and 1000")]
    public int RateLimitPerMinute { get; set; } = 60;

    /// <summary>
    /// Gets or sets the token limit per minute.
    /// </summary>
    [Range(1000, 1000000, ErrorMessage = "TokenLimitPerMinute must be between 1000 and 1000000")]
    public int TokenLimitPerMinute { get; set; } = 90000;

    /// <summary>
    /// Validates the API options.
    /// </summary>
    /// <returns>True if the options are valid; otherwise, false.</returns>
    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(DefaultModel))
        {
            return false;
        }

        if (DefaultTemperature < 0.0 || DefaultTemperature > 2.0)
        {
            return false;
        }

        if (DefaultMaxTokens < 1 || DefaultMaxTokens > 4096)
        {
            return false;
        }

        if (DefaultTimeout <= TimeSpan.Zero)
        {
            return false;
        }

        if (RateLimitPerMinute < 1 || RateLimitPerMinute > 1000)
        {
            return false;
        }

        if (TokenLimitPerMinute < 1000 || TokenLimitPerMinute > 1000000)
        {
            return false;
        }

        if (!AllowedModels.Contains(DefaultModel))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if a model is allowed.
    /// </summary>
    /// <param name="model">The model name to check.</param>
    /// <returns>True if the model is allowed; otherwise, false.</returns>
    public bool IsModelAllowed(string model)
    {
        return !string.IsNullOrWhiteSpace(model) && AllowedModels.Contains(model);
    }

    /// <summary>
    /// Merges command-line options with these defaults.
    /// </summary>
    /// <param name="model">The model from command line.</param>
    /// <param name="temperature">The temperature from command line.</param>
    /// <param name="maxTokens">The max tokens from command line.</param>
    /// <returns>A new ApiOptions instance with merged values.</returns>
    public ApiOptions MergeWithCommandLine(string? model, double? temperature, int? maxTokens)
    {
        return new ApiOptions
        {
            DefaultEffort = DefaultEffort,
            DefaultContext = DefaultContext,
            DefaultTimeout = DefaultTimeout,
            DefaultModel = !string.IsNullOrWhiteSpace(model) ? model : DefaultModel,
            DefaultTemperature = temperature ?? DefaultTemperature,
            DefaultMaxTokens = maxTokens ?? DefaultMaxTokens,
            EnableWebSearchByDefault = EnableWebSearchByDefault,
            StreamResponsesByDefault = StreamResponsesByDefault,
            SystemPrompt = SystemPrompt,
            AllowedModels = new HashSet<string>(AllowedModels),
            CustomHeaders = new Dictionary<string, string>(CustomHeaders),
            RateLimitPerMinute = RateLimitPerMinute,
            TokenLimitPerMinute = TokenLimitPerMinute
        };
    }
}