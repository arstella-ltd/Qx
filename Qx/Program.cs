using System.CommandLine;
using System.CommandLine.Invocation;

namespace Qx;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Qx - Query eXpress: OpenAI-powered intelligent CLI tool for developers")
        {
            TreatUnmatchedTokensAsErrors = false
        };

        // Query argument
        var queryArgument = new Argument<string[]>(
            name: "query",
            description: "The query to send to OpenAI API")
        {
            Arity = ArgumentArity.OneOrMore
        };

        // Options
        var reasoningEffortOption = new Option<string>(
            aliases: new[] { "-e", "--effort" },
            description: "Reasoning effort level (low/medium/high)",
            getDefaultValue: () => "medium");

        var searchContextOption = new Option<string>(
            aliases: new[] { "-c", "--context" },
            description: "Search context size (low/medium/high)",
            getDefaultValue: () => "medium");

        var timeoutOption = new Option<int>(
            aliases: new[] { "-t", "--timeout" },
            description: "Timeout in seconds",
            getDefaultValue: () => 60);

        var noSearchOption = new Option<bool>(
            aliases: new[] { "--no-search" },
            description: "Disable web search",
            getDefaultValue: () => false);

        // Add arguments and options to root command
        rootCommand.AddArgument(queryArgument);
        rootCommand.AddOption(reasoningEffortOption);
        rootCommand.AddOption(searchContextOption);
        rootCommand.AddOption(timeoutOption);
        rootCommand.AddOption(noSearchOption);

        // Handler
        rootCommand.SetHandler(async (InvocationContext context) =>
        {
            var query = context.ParseResult.GetValueForArgument(queryArgument);
            var effort = context.ParseResult.GetValueForOption(reasoningEffortOption);
            var searchContext = context.ParseResult.GetValueForOption(searchContextOption);
            var timeout = context.ParseResult.GetValueForOption(timeoutOption);
            var noSearch = context.ParseResult.GetValueForOption(noSearchOption);

            if (query == null || query.Length == 0)
            {
                await Console.Error.WriteLineAsync("Error: No query provided");
                context.ExitCode = 1;
                return;
            }

            var queryText = string.Join(" ", query);

            // Check for API key
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                await Console.Error.WriteLineAsync("Error: OPENAI_API_KEY environment variable is not set");
                context.ExitCode = 3; // Authentication error
                return;
            }

            // TODO: Implement OpenAI API integration
            await Console.Out.WriteLineAsync($"Query: {queryText}");
            await Console.Out.WriteLineAsync($"Effort: {effort}");
            await Console.Out.WriteLineAsync($"Context: {searchContext}");
            await Console.Out.WriteLineAsync($"Timeout: {timeout}s");
            await Console.Out.WriteLineAsync($"Web Search: {!noSearch}");
            await Console.Out.WriteLineAsync();
            await Console.Out.WriteLineAsync("Note: OpenAI API integration is not yet implemented.");

            context.ExitCode = 0;
        });

        // Version command
        var versionCommand = new Command("version", "Show version information");
        versionCommand.SetHandler(() =>
        {
            Console.WriteLine("qx version 0.1.0-alpha");
            Console.WriteLine("Copyright © 2025 Qx Development Team");
        });
        rootCommand.AddCommand(versionCommand);

        // Parse and execute
        return await rootCommand.InvokeAsync(args);
    }
}
