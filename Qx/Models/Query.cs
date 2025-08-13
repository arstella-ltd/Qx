using System.ComponentModel.DataAnnotations;

namespace Qx.Models;

/// <summary>
/// Represents a query request to the OpenAI API.
/// </summary>
internal sealed class Query
{
    /// <summary>
    /// Gets or sets the content of the query.
    /// </summary>
    [Required(ErrorMessage = "Query content is required")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reasoning effort level for the query.
    /// </summary>
    public EffortLevel ReasoningEffort { get; set; } = EffortLevel.Medium;

    /// <summary>
    /// Gets or sets the search context size for web searches.
    /// </summary>
    public ContextSize SearchContext { get; set; } = ContextSize.Medium;

    /// <summary>
    /// Gets or sets the timeout for the query execution.
    /// </summary>
    [Range(1, 600, ErrorMessage = "Timeout must be between 1 and 600 seconds")]
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Gets or sets the model to use for the query.
    /// </summary>
    public string Model { get; set; } = "gpt-3.5-turbo";

    /// <summary>
    /// Gets or sets the temperature for response generation.
    /// </summary>
    [Range(0.0, 2.0, ErrorMessage = "Temperature must be between 0.0 and 2.0")]
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Gets or sets the maximum number of tokens in the response.
    /// </summary>
    [Range(1, 4096, ErrorMessage = "MaxTokens must be between 1 and 4096")]
    public int MaxTokens { get; set; } = 1000;

    /// <summary>
    /// Gets or sets whether web search is enabled.
    /// </summary>
    public bool EnableWebSearch { get; set; } = true;

    /// <summary>
    /// Creates a new Query instance with the specified content.
    /// </summary>
    /// <param name="content">The query content.</param>
    public Query(string content)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }

    /// <summary>
    /// Default constructor for serialization.
    /// </summary>
    public Query()
    {
    }
}

/// <summary>
/// Represents the reasoning effort level for queries.
/// </summary>
internal enum EffortLevel
{
    /// <summary>
    /// Low reasoning effort - faster responses with less analysis.
    /// </summary>
    Low,

    /// <summary>
    /// Medium reasoning effort - balanced speed and analysis.
    /// </summary>
    Medium,

    /// <summary>
    /// High reasoning effort - thorough analysis with slower responses.
    /// </summary>
    High
}

/// <summary>
/// Represents the search context size for web searches.
/// </summary>
internal enum ContextSize
{
    /// <summary>
    /// Small context size - fewer search results.
    /// </summary>
    Low,

    /// <summary>
    /// Medium context size - moderate number of search results.
    /// </summary>
    Medium,

    /// <summary>
    /// Large context size - extensive search results.
    /// </summary>
    High
}