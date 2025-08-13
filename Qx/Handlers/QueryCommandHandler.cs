using System.CommandLine;
using System.CommandLine.Parsing;
using System.ClientModel;
using Qx.Services;

namespace Qx.Handlers;

internal sealed class QueryCommandHandler
{
    private readonly IOpenAIService _openAIService;

    public QueryCommandHandler(IOpenAIService openAIService)
    {
        _openAIService = openAIService ?? throw new ArgumentNullException(nameof(openAIService));
    }

    public async Task<int> HandleAsync(string[] promptParts, string model, string? outputPath, double temperature, int maxTokens, bool enableWebSearch = true)
    {
        string prompt = string.Join(" ", promptParts ?? Array.Empty<string>());

        if (string.IsNullOrWhiteSpace(prompt))
        {
            await Console.Error.WriteLineAsync("Error: Prompt cannot be empty.").ConfigureAwait(false);
            return 1;
        }

        try
        {
            await Console.Out.WriteLineAsync($"Processing query with model: {model}").ConfigureAwait(false);
            await Console.Out.WriteLineAsync($"Temperature: {temperature}, Max tokens: {maxTokens}").ConfigureAwait(false);
            if (enableWebSearch)
            {
                await Console.Out.WriteLineAsync("Web search: Enabled").ConfigureAwait(false);
            }
            await Console.Out.WriteLineAsync().ConfigureAwait(false);

            string response = await _openAIService.GetCompletionAsync(
                prompt, 
                model, 
                temperature, 
                maxTokens,
                enableWebSearch).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(outputPath))
            {
                await File.WriteAllTextAsync(outputPath, response).ConfigureAwait(false);
                await Console.Out.WriteLineAsync($"Response saved to: {outputPath}").ConfigureAwait(false);
            }
            else
            {
                await Console.Out.WriteLineAsync("Response:").ConfigureAwait(false);
                await Console.Out.WriteLineAsync(response).ConfigureAwait(false);
            }

            return 0;
        }
        catch (ClientResultException ex)
        {
            await Console.Error.WriteLineAsync($"API Error: {ex.Message}").ConfigureAwait(false);
            return 2;
        }
        catch (HttpRequestException ex)
        {
            await Console.Error.WriteLineAsync($"Network Error: {ex.Message}").ConfigureAwait(false);
            return 3;
        }
        catch (TaskCanceledException)
        {
            await Console.Error.WriteLineAsync("Error: Request timed out.").ConfigureAwait(false);
            return 124;
        }
        catch (UnauthorizedAccessException ex)
        {
            await Console.Error.WriteLineAsync($"File Access Error: {ex.Message}").ConfigureAwait(false);
            return 4;
        }
        catch (IOException ex)
        {
            await Console.Error.WriteLineAsync($"I/O Error: {ex.Message}").ConfigureAwait(false);
            return 5;
        }
    }
}