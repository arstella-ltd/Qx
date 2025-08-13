namespace Qx.Services;

/// <summary>
/// Interface for OpenAI API service
/// </summary>
internal interface IOpenAIService
{
    /// <summary>
    /// Get a completion from OpenAI with specific parameters
    /// </summary>
    /// <param name="prompt">The prompt to send</param>
    /// <param name="model">The model to use</param>
    /// <param name="temperature">Temperature for response generation</param>
    /// <param name="maxTokens">Maximum tokens for the response</param>
    /// <param name="enableWebSearch">Whether to enable web search tool</param>
    /// <param name="enableFunctionCalling">Whether to enable function calling</param>
    /// <param name="showFunctionCalls">Whether to show function call indicators in output</param>
    /// <returns>The response from OpenAI</returns>
    Task<string> GetCompletionAsync(string prompt, string model, double temperature, int? maxTokens, bool enableWebSearch = false, bool enableFunctionCalling = false, bool showFunctionCalls = false);
    
    /// <summary>
    /// Get a completion from OpenAI with detailed response information
    /// </summary>
    /// <param name="prompt">The prompt to send</param>
    /// <param name="model">The model to use</param>
    /// <param name="temperature">Temperature for response generation</param>
    /// <param name="maxTokens">Maximum tokens for the response</param>
    /// <param name="enableWebSearch">Whether to enable web search tool</param>
    /// <param name="enableFunctionCalling">Whether to enable function calling</param>
    /// <param name="showFunctionCalls">Whether to show function call indicators in output</param>
    /// <returns>A tuple of response text and detailed response object</returns>
    Task<(string response, ResponseDetails? details)> GetCompletionWithDetailsAsync(string prompt, string model, double temperature, int? maxTokens, bool enableWebSearch = false, bool enableFunctionCalling = false, bool showFunctionCalls = false);
}