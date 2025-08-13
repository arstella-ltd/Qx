using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Qx.Handlers;
using Qx.Services;

namespace Qx.Commands;

internal static class CommandRegistry
{
    public static RootCommand CreateRootCommand(IServiceProvider serviceProvider)
    {
        var rootCommand = new RootCommand("Qx - Query eXpress: OpenAI-powered intelligent CLI tool for developers");
        
        var openAIService = serviceProvider.GetRequiredService<IOpenAIService>();
        
        // Main prompt argument for direct usage: qx "prompt"
        var promptArgument = new Argument<string>("prompt")
        {
            Description = "The natural language prompt to send",
            Arity = ArgumentArity.ZeroOrOne
        };
        
        // Options
        var modelOption = new Option<string>("--model")
        {
            Description = "The AI model to use"
        };
        modelOption.Aliases.Add("-m");
        
        var outputOption = new Option<string>("--output")
        {
            Description = "Output file path"
        };
        outputOption.Aliases.Add("-o");
        
        var temperatureOption = new Option<double>("--temperature")
        {
            Description = "Temperature for response generation (0.0 to 2.0)"
        };
        temperatureOption.Aliases.Add("-t");
        
        var maxTokensOption = new Option<int?>("--max-tokens")
        {
            Description = "Maximum number of tokens in the response (unlimited if not specified)"
        };
        
        var webSearchOption = new Option<bool>("--web-search")
        {
            Description = "Enable web search for more comprehensive answers"
        };
        webSearchOption.Aliases.Add("-w");
        
        var noWebSearchOption = new Option<bool>("--no-web-search")
        {
            Description = "Disable web search (use model knowledge only)"
        };
        
        var functionsOption = new Option<bool>("--functions")
        {
            Description = "Enable function calling (GetCurrentTime, GetWeather, CalculateExpression)"
        };
        functionsOption.Aliases.Add("-f");
        
        var noFunctionsOption = new Option<bool>("--no-functions")
        {
            Description = "Disable function calling"
        };
        
        var verboseOption = new Option<bool>("--verbose")
        {
            Description = "Show detailed response information in JSON format"
        };
        verboseOption.Aliases.Add("-v");
        
        var versionOption = new Option<bool>("--version")
        {
            Description = "Show version information"
        };
        
        // Add argument and options to root command
        rootCommand.Arguments.Add(promptArgument);
        rootCommand.Options.Add(modelOption);
        rootCommand.Options.Add(outputOption);
        rootCommand.Options.Add(temperatureOption);
        rootCommand.Options.Add(maxTokensOption);
        rootCommand.Options.Add(webSearchOption);
        rootCommand.Options.Add(noWebSearchOption);
        rootCommand.Options.Add(functionsOption);
        rootCommand.Options.Add(noFunctionsOption);
        rootCommand.Options.Add(verboseOption);
        rootCommand.Options.Add(versionOption);
        
        // Set handler for the root command
        rootCommand.SetAction((parseResult) =>
        {
            // Handle version flag
            if (parseResult.GetValue(versionOption))
            {
                Console.WriteLine("qx version 0.1.0-alpha");
                Console.WriteLine("Copyright © 2025 Qx Development Team");
                return 0;
            }
            
            string? prompt = parseResult.GetValue(promptArgument);
            
            // If no prompt provided, show help
            if (string.IsNullOrEmpty(prompt))
            {
                Console.WriteLine("Usage: qx <prompt> [options]");
                Console.WriteLine("\nRun a query against OpenAI API.");
                Console.WriteLine("\nArguments:");
                Console.WriteLine("  <prompt>  The natural language prompt to send");
                Console.WriteLine("\nOptions:");
                Console.WriteLine("  -m, --model <model>          The AI model to use [default: gpt-5]");
                Console.WriteLine("  -o, --output <output>        Output file path");
                Console.WriteLine("  -t, --temperature <temperature>  Temperature for response generation (0.0 to 2.0) [default: 1]");
                Console.WriteLine("  --max-tokens <max-tokens>    Maximum number of tokens in the response (unlimited if not specified)");
                Console.WriteLine("  -w, --web-search             Enable web search for more comprehensive answers [default: enabled]");
                Console.WriteLine("  --no-web-search              Disable web search (use model knowledge only)");
                Console.WriteLine("  -f, --functions              Enable function calling (GetCurrentTime, GetWeather, CalculateExpression) [default: enabled]");
                Console.WriteLine("  --no-functions               Disable function calling");
                Console.WriteLine("  -v, --verbose                Show detailed response information in JSON format");
                Console.WriteLine("  --version                    Show version information");
                Console.WriteLine("  -h, --help                   Show help and usage information");
                return 0;
            }
            
            // Execute the query
            string model = parseResult.GetValue(modelOption) ?? "gpt-5";
            string? output = parseResult.GetValue(outputOption);
            double temperature = parseResult.GetValue(temperatureOption);
            if (temperature == 0)
            {
                temperature = 1.0;
            }
            int? maxTokens = parseResult.GetValue(maxTokensOption);
            
            // Determine web search setting
            bool enableWebSearch = !parseResult.GetValue(noWebSearchOption);
            if (parseResult.GetValue(webSearchOption))
            {
                enableWebSearch = true;
            }
            
            // Determine function calling setting
            bool enableFunctionCalling = !parseResult.GetValue(noFunctionsOption);
            if (parseResult.GetValue(functionsOption))
            {
                enableFunctionCalling = true;
            }
            
            bool verbose = parseResult.GetValue(verboseOption);
            
            var handler = new QueryCommandHandler(openAIService);
            Task.Run(async () => await handler.HandleAsync(
                new[] { prompt }, 
                model, 
                output, 
                temperature, 
                maxTokens,
                enableWebSearch,
                enableFunctionCalling,
                verbose).ConfigureAwait(false)
            ).GetAwaiter().GetResult();
            
            return 0;
        });
        
        return rootCommand;
    }
}