using System.CommandLine;
using System.CommandLine.Parsing;
using System.ClientModel;
using System.Text.Json;
using Qx.Services;

namespace Qx.Handlers;

internal sealed class QueryCommandHandler
{
    private readonly IOpenAIService _openAIService;

    public QueryCommandHandler(IOpenAIService openAIService)
    {
        _openAIService = openAIService ?? throw new ArgumentNullException(nameof(openAIService));
    }

    public async Task<int> HandleAsync(string[] promptParts, string model, string? outputPath, double temperature, int? maxTokens, bool enableWebSearch = true, bool enableFunctionCalling = true, bool verbose = false)
    {
        string prompt = string.Join(" ", promptParts ?? Array.Empty<string>());

        if (string.IsNullOrWhiteSpace(prompt))
        {
            await Console.Error.WriteLineAsync("Error: Prompt cannot be empty.").ConfigureAwait(false);
            return 1;
        }

        try
        {
            // Only show verbose information if requested
            if (verbose)
            {
                await Console.Out.WriteLineAsync($"Processing query with model: {model}").ConfigureAwait(false);
                string maxTokensDisplay = maxTokens.HasValue ? maxTokens.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "unlimited";
                await Console.Out.WriteLineAsync($"Temperature: {temperature}, Max tokens: {maxTokensDisplay}").ConfigureAwait(false);
                if (enableWebSearch)
                {
                    await Console.Out.WriteLineAsync("Web search: Enabled (Note: Not all models support web search)").ConfigureAwait(false);
                }
                if (enableFunctionCalling)
                {
                    await Console.Out.WriteLineAsync("Function calling: Enabled (GetCurrentTime, GetWeather, CalculateExpression)").ConfigureAwait(false);
                }
                await Console.Out.WriteLineAsync().ConfigureAwait(false);
            }

            // Get response with optional verbose details
            var (response, responseDetails) = await _openAIService.GetCompletionWithDetailsAsync(
                prompt, 
                model, 
                temperature, 
                maxTokens,
                enableWebSearch,
                enableFunctionCalling,
                verbose).ConfigureAwait(false);
            
            // Show verbose output if requested
            if (verbose && responseDetails != null)
            {
                await Console.Out.WriteLineAsync("\n=== Response Details (JSON) ===").ConfigureAwait(false);
                // Use source-generated JSON serializer for AOT compatibility
                string jsonOutput = JsonSerializer.Serialize(responseDetails, ResponseDetailsJsonContext.Default.ResponseDetails);
                await Console.Out.WriteLineAsync(jsonOutput).ConfigureAwait(false);
                await Console.Out.WriteLineAsync("\n=== Response Text ===").ConfigureAwait(false);
            }

            if (!string.IsNullOrEmpty(outputPath))
            {
                await File.WriteAllTextAsync(outputPath, response).ConfigureAwait(false);
                await Console.Out.WriteLineAsync($"Response saved to: {outputPath}").ConfigureAwait(false);
            }
            else
            {
                // Only show "Response:" label in verbose mode
                if (verbose)
                {
                    await Console.Out.WriteLineAsync("Response:").ConfigureAwait(false);
                }
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