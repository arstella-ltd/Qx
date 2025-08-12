using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Qx.Services;

[assembly: CLSCompliant(false)]
[assembly: InternalsVisibleTo("Qx.Tests")]

namespace Qx;

internal sealed class Program
{
    public static int Main(string[] args)
    {
        var rootCommand = new RootCommand("Qx - Query eXpress: OpenAI-powered intelligent CLI tool for developers");

        // Query argument
        var queryArgument = new Argument<string[]>("query")
        {
            Description = "The query to send to OpenAI API",
            Arity = ArgumentArity.ZeroOrMore
        };

        // Options
        var reasoningEffortOption = new Option<string>("--effort")
        {
            Description = "Reasoning effort level (low/medium/high)"
        };
        reasoningEffortOption.Aliases.Add("-e");

        var searchContextOption = new Option<string>("--context")
        {
            Description = "Search context size (low/medium/high)"
        };
        searchContextOption.Aliases.Add("-c");

        var timeoutOption = new Option<int>("--timeout")
        {
            Description = "Timeout in seconds"
        };
        timeoutOption.Aliases.Add("-t");

        var noSearchOption = new Option<bool>("--no-search")
        {
            Description = "Disable web search"
        };
        
        var streamOption = new Option<bool>("--stream")
        {
            Description = "Stream the response"
        };
        streamOption.Aliases.Add("-s");

        // Add arguments and options to root command
        rootCommand.Arguments.Add(queryArgument);
        rootCommand.Options.Add(reasoningEffortOption);
        rootCommand.Options.Add(searchContextOption);
        rootCommand.Options.Add(timeoutOption);
        rootCommand.Options.Add(noSearchOption);
        rootCommand.Options.Add(streamOption);

        // Handler - using simple action
        rootCommand.SetAction((parseResult) =>
        {
            string[] query = parseResult.GetValue(queryArgument) ?? Array.Empty<string>();
            string effort = parseResult.GetValue(reasoningEffortOption) ?? "medium";
            string searchContext = parseResult.GetValue(searchContextOption) ?? "medium";
            int timeout = parseResult.GetValue(timeoutOption);
            if (timeout == 0) 
            {
                timeout = 60;
            }
            bool noSearch = parseResult.GetValue(noSearchOption);
            bool stream = parseResult.GetValue(streamOption);

            if (query.Length == 0)
            {
                // No query provided - show help by default
                return 0;
            }

            string queryText = string.Join(" ", query);

            // Check for API key
            string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.Error.WriteLine("Error: OPENAI_API_KEY environment variable is not set");
                return 3; // Authentication error
            }

            // Create OpenAI service
            var openAIService = new OpenAIService(apiKey);
            
            // Configure query options
            var options = new QueryOptions
            {
                EffortLevel = effort,
                ContextSize = searchContext,
                TimeoutSeconds = timeout,
                EnableWebSearch = !noSearch
            };
            
            try
            {
                if (stream)
                {
                    // Stream the response
                    IAsyncEnumerable<string> responseStream = openAIService.QueryStreamAsync(queryText, options);
                    IAsyncEnumerator<string> enumerator = responseStream.GetAsyncEnumerator();
                    
                    while (enumerator.MoveNextAsync().AsTask().GetAwaiter().GetResult())
                    {
                        Console.Write(enumerator.Current);
                    }
                    Console.WriteLine();
                }
                else
                {
                    // Send query and display response
                    string response = openAIService.QueryAsync(queryText, options).GetAwaiter().GetResult();
                    Console.WriteLine(response);
                }
                return 0;
            }
            catch (TimeoutException ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 124; // Timeout error code
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1; // General error
            }
        });

        // Version command
        var versionCommand = new Command("version", "Show version information");
        versionCommand.SetAction((parseResult) =>
        {
            Console.WriteLine("qx version 0.1.0-alpha");
            Console.WriteLine("Copyright © 2025 Qx Development Team");
            return 0;
        });
        rootCommand.Subcommands.Add(versionCommand);

        // Parse and execute
        return rootCommand.Parse(args).Invoke();
    }
}