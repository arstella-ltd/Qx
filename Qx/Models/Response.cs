namespace Qx.Models;

/// <summary>
/// Represents a response from the OpenAI API.
/// </summary>
internal sealed class Response
{
    /// <summary>
    /// Gets or sets the content of the response.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the metadata associated with the response.
    /// </summary>
    public ResponseMetadata Metadata { get; set; } = new();

    /// <summary>
    /// Gets or sets whether the response is complete.
    /// </summary>
    public bool IsComplete { get; set; }

    /// <summary>
    /// Gets or sets the error message if the response failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets whether the response was successful.
    /// </summary>
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage) && IsComplete;

    /// <summary>
    /// Creates a successful response with the specified content.
    /// </summary>
    /// <param name="content">The response content.</param>
    /// <returns>A successful response.</returns>
    public static Response Success(string content)
    {
        return new Response
        {
            Content = content ?? string.Empty,
            IsComplete = true
        };
    }

    /// <summary>
    /// Creates a failed response with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed response.</returns>
    public static Response Failure(string errorMessage)
    {
        return new Response
        {
            ErrorMessage = errorMessage,
            IsComplete = true
        };
    }
}

/// <summary>
/// Represents metadata about a response.
/// </summary>
internal sealed class ResponseMetadata
{
    /// <summary>
    /// Gets or sets the model used to generate the response.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of tokens in the prompt.
    /// </summary>
    public int PromptTokens { get; set; }

    /// <summary>
    /// Gets or sets the number of tokens in the completion.
    /// </summary>
    public int CompletionTokens { get; set; }

    /// <summary>
    /// Gets or sets the total number of tokens used.
    /// </summary>
    public int TotalTokens { get; set; }

    /// <summary>
    /// Gets or sets the time taken to generate the response.
    /// </summary>
    public TimeSpan ResponseTime { get; set; }

    /// <summary>
    /// Gets or sets whether web search was used.
    /// </summary>
    public bool WebSearchUsed { get; set; }

    /// <summary>
    /// Gets or sets the number of web search results used.
    /// </summary>
    public int WebSearchResultsCount { get; set; }

    /// <summary>
    /// Gets or sets the finish reason for the response.
    /// </summary>
    public string FinishReason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the response was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
