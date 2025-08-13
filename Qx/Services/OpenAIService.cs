using System.Runtime.CompilerServices;
using OpenAI;
using OpenAI.Chat;

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
    public async Task<string> GetCompletionAsync(string prompt, string model, double temperature, int maxTokens)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);

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