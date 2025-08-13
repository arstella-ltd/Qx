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
        string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.Error.WriteLine("Error: OPENAI_API_KEY environment variable is not set");
            return 3;
        }

        ServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IOpenAIService>(new OpenAIService(apiKey));
        
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        
        RootCommand rootCommand = CommandRegistry.CreateRootCommand(serviceProvider);
        
        return rootCommand.Parse(args).Invoke();
    }
}