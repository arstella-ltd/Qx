namespace Qx.Services;

/// <summary>
/// Interface for OpenAI API service
/// </summary>
internal interface IOpenAIService
{
    /// <summary>
    /// Send a query to OpenAI and get a response
    /// </summary>
    /// <param name="query">The query to send</param>
    /// <param name="options">Query options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response from OpenAI</returns>
    Task<string> QueryAsync(string query, QueryOptions options, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send a query to OpenAI and stream the response
    /// </summary>
    /// <param name="query">The query to send</param>
    /// <param name="options">Query options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async enumerable of response chunks</returns>
    IAsyncEnumerable<string> QueryStreamAsync(string query, QueryOptions options, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get a completion from OpenAI with specific parameters
    /// </summary>
    /// <param name="prompt">The prompt to send</param>
    /// <param name="model">The model to use</param>
    /// <param name="temperature">Temperature for response generation</param>
    /// <param name="maxTokens">Maximum tokens for the response</param>
    /// <param name="enableWebSearch">Whether to enable web search tool</param>
    /// <returns>The response from OpenAI</returns>
    Task<string> GetCompletionAsync(string prompt, string model, double temperature, int maxTokens, bool enableWebSearch = false);
    
    /// <summary>
    /// Get a completion from OpenAI with detailed response information
    /// </summary>
    /// <param name="prompt">The prompt to send</param>
    /// <param name="model">The model to use</param>
    /// <param name="temperature">Temperature for response generation</param>
    /// <param name="maxTokens">Maximum tokens for the response</param>
    /// <param name="enableWebSearch">Whether to enable web search tool</param>
    /// <returns>A tuple of response text and detailed response object</returns>
    Task<(string response, ResponseDetails? details)> GetCompletionWithDetailsAsync(string prompt, string model, double temperature, int maxTokens, bool enableWebSearch = false);
}

/// <summary>
/// Options for querying OpenAI
/// </summary>
internal record QueryOptions
{
    /// <summary>
    /// The effort level for reasoning (low, medium, high)
    /// </summary>
    public string EffortLevel { get; init; } = "medium";
    
    /// <summary>
    /// The context size (low, medium, high)
    /// </summary>
    public string ContextSize { get; init; } = "medium";
    
    /// <summary>
    /// Whether to enable web search
    /// </summary>
    public bool EnableWebSearch { get; init; } = true;
    
    /// <summary>
    /// Timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; init; } = 60;
    
    /// <summary>
    /// Model to use (e.g., "gpt-4", "gpt-3.5-turbo")
    /// </summary>
    public string Model { get; init; } = "gpt-4o-mini";
    
    /// <summary>
    /// Maximum tokens for the response
    /// </summary>
    public int? MaxTokens { get; init; }
    
    /// <summary>
    /// Temperature for response generation (0.0 to 2.0)
    /// </summary>
    public double Temperature { get; init; } = 0.7;
    
    /// <summary>
    /// System prompt to use
    /// </summary>
    public string? SystemPrompt { get; init; }
}