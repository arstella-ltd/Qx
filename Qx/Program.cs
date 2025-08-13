using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Qx.Commands;
using Qx.Services;

[assembly: CLSCompliant(false)]
[assembly: InternalsVisibleTo("Qx.Tests")]

namespace Qx;

internal sealed class Program
{
    public static int Main(string[] args)
    {
        // Check if help or version is requested
        if (args.Length > 0 && (args[0] == "--help" || args[0] == "-h" || args[0] == "--version" || args[0] == "version"))
        {
            // For help and version, we don't need the API key
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IOpenAIService>(new OpenAIService("dummy-key-for-help"));
            
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            RootCommand rootCommand = CommandRegistry.CreateRootCommand(serviceProvider);
            
            return rootCommand.Parse(args).Invoke();
        }

        // For other commands, check API key
        string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        
        // If no arguments provided, show help
        if (args.Length == 0)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IOpenAIService>(new OpenAIService(apiKey ?? "dummy-key-for-help"));
            
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            RootCommand rootCommand = CommandRegistry.CreateRootCommand(serviceProvider);
            
            return rootCommand.Parse(new[] { "--help" }).Invoke();
        }
        
        // For actual queries, require API key
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.Error.WriteLine("Error: OPENAI_API_KEY environment variable is not set");
            return 3;
        }

        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IOpenAIService>(new OpenAIService(apiKey));
            
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            RootCommand rootCommand = CommandRegistry.CreateRootCommand(serviceProvider);
            
            return rootCommand.Parse(args).Invoke();
        }
    }
}