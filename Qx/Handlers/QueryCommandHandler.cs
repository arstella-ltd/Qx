using System.CommandLine;
using System.CommandLine.Parsing;
using Qx.Services;

namespace Qx.Handlers;

internal sealed class QueryCommandHandler
{
    private readonly IOpenAIService _openAIService;

    public QueryCommandHandler(IOpenAIService openAIService)
    {
        _openAIService = openAIService ?? throw new ArgumentNullException(nameof(openAIService));
    }

    public async Task<int> HandleAsync(string[] promptParts, string model, string? outputPath, double temperature, int maxTokens)
    {
        string prompt = string.Join(" ", promptParts ?? Array.Empty<string>());

        if (string.IsNullOrWhiteSpace(prompt))
        {
            Console.WriteLine("Error: Prompt cannot be empty.");
            return 1;
        }

        try
        {
            Console.WriteLine($"Processing query with model: {model}");
            Console.WriteLine($"Temperature: {temperature}, Max tokens: {maxTokens}");
            Console.WriteLine();

            string response = await _openAIService.GetCompletionAsync(
                prompt, 
                model, 
                temperature, 
                maxTokens).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(outputPath))
            {
                await File.WriteAllTextAsync(outputPath, response).ConfigureAwait(false);
                Console.WriteLine($"Response saved to: {outputPath}");
            }
            else
            {
                Console.WriteLine("Response:");
                Console.WriteLine(response);
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
}