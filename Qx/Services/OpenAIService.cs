using System.Runtime.CompilerServices;
using OpenAI;
using OpenAI.Chat;
#pragma warning disable OPENAI001 // Type is for evaluation purposes only
using OpenAI.Responses;
#pragma warning restore OPENAI001
using System.Text.Json;

namespace Qx.Services;

/// <summary>
/// Implementation of OpenAI API service
/// </summary>
internal sealed class OpenAIService : IOpenAIService
{
    private readonly ChatClient _chatClient;
    private readonly string _apiKey;
    private readonly ToolService _toolService;

    /// <summary>
    /// Initialize a new instance of OpenAIService
    /// </summary>
    /// <param name="apiKey">OpenAI API key</param>
    public OpenAIService(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        _apiKey = apiKey;
        _toolService = new ToolService();
        
        // Create OpenAI client with the API key
        var openAIClient = new OpenAIClient(apiKey);
        _chatClient = openAIClient.GetChatClient("gpt-4o-mini");
    }

    /// <summary>
    /// Get a completion from OpenAI with specific parameters
    /// </summary>
#pragma warning disable OPENAI001 // Type is for evaluation purposes only
    public async Task<string> GetCompletionAsync(string prompt, string model, double temperature, int? maxTokens, bool enableWebSearch = false, bool enableFunctionCalling = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);

        // Always use OpenAIResponseClient for consistent API usage
        // This allows web search and function calling on all models that support them
        var responseClient = new OpenAIResponseClient(
            model: model,
            apiKey: _apiKey);

        var options = new ResponseCreationOptions
        {
            Temperature = (float)temperature,
            MaxOutputTokenCount = maxTokens
        };

        // Add web search tool
        if (enableWebSearch)
        {
            options.Tools.Add(ResponseTool.CreateWebSearchTool());
        }
        
        // Add function calling tools
        if (enableFunctionCalling)
        {
            foreach (var tool in _toolService.GetAvailableTools())
            {
                options.Tools.Add(tool);
            }
        }

        var response = await responseClient.CreateResponseAsync(
            userInputText: prompt,
            options).ConfigureAwait(false);

        // Process response items and extract text
        var resultText = new System.Text.StringBuilder();
        
        // Debug: Log the number of output items
        if (response.Value.OutputItems == null || response.Value.OutputItems.Count == 0)
        {
            // If no output items, try to get content directly
            return "No response generated. Web search may not be available for this model.";
        }
        
        bool hasTextContent = false;
        var debugInfo = new System.Text.StringBuilder();
        
        foreach (ResponseItem item in response.Value.OutputItems ?? new List<ResponseItem>())
        {
            if (item is MessageResponseItem messageItem)
            {
                // MessageResponseItem should have Content property with TextContent items
                if (messageItem.Content != null && messageItem.Content.Count > 0)
                {
                    foreach (var contentItem in messageItem.Content)
                    {
                        if (contentItem?.Text != null)
                        {
                            resultText.Append(contentItem.Text);
                            hasTextContent = true;
                        }
                    }
                }
                else
                {
                    // Debug: log if content is null or empty
                    debugInfo.AppendLine($"[Debug: MessageItem with null/empty content]");
                }
            }
            else if (item is WebSearchCallResponseItem webSearchItem)
            {
                // Note: Some models may only return WebSearchCallResponseItems without message content
                // This might be a limitation of certain models with web search
                debugInfo.AppendLine($"[Web search: {webSearchItem.Status}]");
            }
            else if (item is FunctionCallResponseItem functionItem)
            {
                // Execute the function and append the result
                string functionResult = _toolService.ExecuteFunction(functionItem.FunctionName, functionItem.FunctionArguments);
                resultText.AppendLine($"\n[Function Call: {functionItem.FunctionName}]");
                resultText.AppendLine(functionResult);
                hasTextContent = true;
            }
            else
            {
                // Debug: log unknown item types
                debugInfo.AppendLine($"[Debug: Unknown item type: {item?.GetType().Name}]");
            }
        }
        
        // If we got no text content, return what we have or an error message
        if (!hasTextContent)
        {
            if (debugInfo.Length > 0)
            {
                return $"Web search was attempted but no text response was generated. Try using --model gpt-4o-mini for web search, or --no-web-search for this model.\nDebug: {debugInfo}";
            }
            return "No response content available.";
        }

        string result = resultText.ToString();
        return string.IsNullOrWhiteSpace(result) 
            ? "No response content available. Try using a different model or disabling web search." 
            : result;
    }
#pragma warning restore OPENAI001

    /// <summary>
    /// Get a completion from OpenAI with detailed response information
    /// </summary>
#pragma warning disable OPENAI001 // Type is for evaluation purposes only
    public async Task<(string response, ResponseDetails? details)> GetCompletionWithDetailsAsync(string prompt, string model, double temperature, int? maxTokens, bool enableWebSearch = false, bool enableFunctionCalling = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);

        // Always use OpenAIResponseClient for consistent API usage
        // This allows web search and function calling on all models that support them
        var responseClient = new OpenAIResponseClient(
            model: model,
            apiKey: _apiKey);

        var options = new ResponseCreationOptions
        {
            Temperature = (float)temperature,
            MaxOutputTokenCount = maxTokens
        };

        // Add web search tool
        if (enableWebSearch)
        {
            options.Tools.Add(ResponseTool.CreateWebSearchTool());
        }
        
        // Add function calling tools
        if (enableFunctionCalling)
        {
            foreach (var tool in _toolService.GetAvailableTools())
            {
                options.Tools.Add(tool);
            }
        }

        var response = await responseClient.CreateResponseAsync(
            userInputText: prompt,
            options).ConfigureAwait(false);

        // Create detailed response object
        var details = new ResponseDetails
        {
            Model = model,
            RequestOptions = new RequestOptions
            {
                Temperature = temperature,
                MaxTokens = maxTokens,
                WebSearchEnabled = enableWebSearch
            },
            ResponseMetadata = new ResponseMetadata
            {
                OutputItemsCount = response.Value.OutputItems?.Count ?? 0,
                Items = response.Value.OutputItems?.Select(item => new ItemInfo
                {
                    Type = item.GetType().Name,
                    Content = item is MessageResponseItem msg ? new ContentInfo
                    {
                        ContentCount = msg.Content?.Count ?? 0,
                        TextPreviews = msg.Content?.Select(c => c.Text?.Length > 100 ? c.Text[..100] + "..." : c.Text).ToList()
                    } : null,
                    WebSearchStatus = item is WebSearchCallResponseItem ws ? ws.Status?.ToString() : null,
                    FunctionName = item is FunctionCallResponseItem fc ? fc.FunctionName : null
                }).ToList()
            }
        };

        // Process response items and extract text
        var resultText = new System.Text.StringBuilder();
        bool hasTextContent = false;
        
        foreach (ResponseItem item in response.Value.OutputItems ?? new List<ResponseItem>())
        {
            if (item is MessageResponseItem messageItem)
            {
                if (messageItem.Content != null && messageItem.Content.Count > 0)
                {
                    foreach (var contentItem in messageItem.Content)
                    {
                        if (contentItem?.Text != null)
                        {
                            resultText.Append(contentItem.Text);
                            hasTextContent = true;
                        }
                    }
                }
            }
            else if (item is FunctionCallResponseItem functionItem)
            {
                // Execute the function and append the result
                string functionResult = _toolService.ExecuteFunction(functionItem.FunctionName, functionItem.FunctionArguments);
                resultText.AppendLine($"\n[Function Call: {functionItem.FunctionName}]");
                resultText.AppendLine(functionResult);
                hasTextContent = true;
            }
        }
        
        string result = hasTextContent ? resultText.ToString() : "No response content available.";
        return (result, details);
    }
#pragma warning restore OPENAI001
}