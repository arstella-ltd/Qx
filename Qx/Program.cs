using System.CommandLine;
using System.Diagnostics.CodeAnalysis;

[assembly: CLSCompliant(false)]

namespace Qx;

public sealed class Program
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

        // Add arguments and options to root command
        rootCommand.Arguments.Add(queryArgument);
        rootCommand.Options.Add(reasoningEffortOption);
        rootCommand.Options.Add(searchContextOption);
        rootCommand.Options.Add(timeoutOption);
        rootCommand.Options.Add(noSearchOption);

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

            // TODO: Implement OpenAI API integration
            Console.WriteLine($"Query: {queryText}");
            Console.WriteLine($"Effort: {effort}");
            Console.WriteLine($"Context: {searchContext}");
            Console.WriteLine($"Timeout: {timeout}s");
            Console.WriteLine($"Web Search: {!noSearch}");
            Console.WriteLine();
            Console.WriteLine("Note: OpenAI API integration is not yet implemented.");
            return 0;
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