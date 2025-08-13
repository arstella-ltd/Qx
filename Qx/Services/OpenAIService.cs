using System.Runtime.CompilerServices;
using OpenAI;
using OpenAI.Chat;
#pragma warning disable OPENAI001 // Type is for evaluation purposes only
using OpenAI.Responses;
#pragma warning restore OPENAI001

namespace Qx.Services;

/// <summary>
/// Implementation of OpenAI API service
/// </summary>
internal sealed class OpenAIService : IOpenAIService
{
    private readonly ChatClient _chatClient;
    private readonly string _apiKey;

    /// <summary>
    /// Initialize a new instance of OpenAIService
    /// </summary>
    /// <param name="apiKey">OpenAI API key</param>
    public OpenAIService(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        _apiKey = apiKey;
        
        // Create OpenAI client with the API key
        var openAIClient = new OpenAIClient(apiKey);
        _chatClient = openAIClient.GetChatClient("gpt-4o-mini");
    }

    /// <summary>
    /// Send a query to OpenAI and get a response
    /// </summary>
    public async Task<string> QueryAsync(string query, QueryOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        ArgumentNullException.ThrowIfNull(options);

        try
        {
            // Create chat client for the specified model
            var openAIClient = new OpenAIClient(_apiKey);
            var chatClient = openAIClient.GetChatClient(options.Model);

            // Build the messages
            var messages = new List<ChatMessage>();
            
            // Add system prompt if provided
            if (!string.IsNullOrWhiteSpace(options.SystemPrompt))
            {
                messages.Add(new SystemChatMessage(options.SystemPrompt));
            }
            else
            {
                // Default system prompt based on effort level
                string systemPrompt = GetSystemPrompt(options.EffortLevel, options.ContextSize);
                messages.Add(new SystemChatMessage(systemPrompt));
            }
            
            // Add user query
            messages.Add(new UserChatMessage(query));

            // Configure chat completion options
            var chatOptions = new ChatCompletionOptions
            {
                Temperature = (float)options.Temperature,
                MaxOutputTokenCount = options.MaxTokens,
            };

            // Add web search tool if enabled
            // Note: Web search is currently only available on specific models and may require
            // special access. For now, we'll use the standard implementation without web search.
            // TODO: Implement web search when available in the SDK
            if (options.EnableWebSearch)
            {
                // Web search will be enabled automatically by the model if supported
                // No explicit tool configuration needed for now
            }

            // Apply timeout
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(options.TimeoutSeconds));

            // Send the request
            var response = await chatClient.CompleteChatAsync(
                messages, 
                chatOptions, 
                cts.Token).ConfigureAwait(false);

            // Return the response text
            return response.Value.Content[0].Text ?? string.Empty;
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException($"Query timed out after {options.TimeoutSeconds} seconds");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to query OpenAI: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Send a query to OpenAI and stream the response
    /// </summary>
    public async IAsyncEnumerable<string> QueryStreamAsync(
        string query, 
        QueryOptions options, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        ArgumentNullException.ThrowIfNull(options);

        // Create chat client for the specified model
        var openAIClient = new OpenAIClient(_apiKey);
        var chatClient = openAIClient.GetChatClient(options.Model);

        // Build the messages
        var messages = new List<ChatMessage>();
        
        // Add system prompt
        if (!string.IsNullOrWhiteSpace(options.SystemPrompt))
        {
            messages.Add(new SystemChatMessage(options.SystemPrompt));
        }
        else
        {
            string systemPrompt = GetSystemPrompt(options.EffortLevel, options.ContextSize);
            messages.Add(new SystemChatMessage(systemPrompt));
        }
        
        // Add user query
        messages.Add(new UserChatMessage(query));

        // Configure chat completion options
        var chatOptions = new ChatCompletionOptions
        {
            Temperature = (float)options.Temperature,
            MaxOutputTokenCount = options.MaxTokens,
        };

        // Add web search tool if enabled
        // Note: Web search is currently only available on specific models and may require
        // special access. For now, we'll use the standard implementation without web search.
        // TODO: Implement web search when available in the SDK
        if (options.EnableWebSearch)
        {
            // Web search will be enabled automatically by the model if supported
            // No explicit tool configuration needed for now
        }

        // Apply timeout
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(options.TimeoutSeconds));

        // Stream the response
        await foreach (var update in chatClient.CompleteChatStreamingAsync(
            messages, 
            chatOptions, 
            cts.Token).ConfigureAwait(false))
        {
            if (update.ContentUpdate.Count > 0)
            {
                string? text = update.ContentUpdate[0].Text;
                if (!string.IsNullOrEmpty(text))
                {
                    yield return text;
                }
            }
        }
    }

    /// <summary>
    /// Get a completion from OpenAI with specific parameters
    /// </summary>
#pragma warning disable OPENAI001 // Type is for evaluation purposes only
    public async Task<string> GetCompletionAsync(string prompt, string model, double temperature, int maxTokens, bool enableWebSearch = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);

        // Use OpenAIResponseClient for web search support
        // Note: Web search works best with specific models like gpt-4o-mini
        // For other models, we'll fallback to regular chat if web search fails
        if (enableWebSearch && (model.Contains("gpt-4o", StringComparison.OrdinalIgnoreCase) || model.Contains("o1", StringComparison.OrdinalIgnoreCase)))
        {
            var responseClient = new OpenAIResponseClient(
                model: model,
                apiKey: _apiKey);

            var options = new ResponseCreationOptions
            {
                Temperature = (float)temperature,
                MaxOutputTokenCount = maxTokens
            };

            // Add web search tool
            options.Tools.Add(ResponseTool.CreateWebSearchTool());

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
            
            foreach (ResponseItem item in response.Value.OutputItems)
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
        else
        {
            // Use regular ChatClient without web search
            var openAIClient = new OpenAIClient(_apiKey);
            var chatClient = openAIClient.GetChatClient(model);

            var messages = new List<ChatMessage>
            {
                new UserChatMessage(prompt)
            };

            var chatOptions = new ChatCompletionOptions
            {
                Temperature = (float)temperature,
                MaxOutputTokenCount = maxTokens
            };

            var completion = await chatClient.CompleteChatAsync(messages, chatOptions).ConfigureAwait(false);
            return completion.Value.Content[0].Text ?? string.Empty;
        }
    }
#pragma warning restore OPENAI001

    /// <summary>
    /// Get the system prompt based on effort level and context size
    /// </summary>
    private static string GetSystemPrompt(string effortLevel, string contextSize)
    {
        string effortDescription = string.Equals(effortLevel, "low", StringComparison.OrdinalIgnoreCase)
            ? "Be concise and direct. Provide quick answers."
            : string.Equals(effortLevel, "high", StringComparison.OrdinalIgnoreCase)
            ? "Think step by step. Provide detailed and thorough analysis."
            : "Provide balanced and clear responses.";

        string contextDescription = string.Equals(contextSize, "low", StringComparison.OrdinalIgnoreCase)
            ? "Focus on the immediate question only."
            : string.Equals(contextSize, "high", StringComparison.OrdinalIgnoreCase)
            ? "Consider broader context and implications."
            : "Consider relevant context as needed.";

        return $"You are a helpful AI assistant. {effortDescription} {contextDescription}";
    }
}