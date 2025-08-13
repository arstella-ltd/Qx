using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Qx.Services;

namespace Qx.Commands;

internal static class CommandRegistry
{
    public static RootCommand CreateRootCommand(IServiceProvider serviceProvider)
    {
        var rootCommand = new RootCommand("Qx - Query eXpress: OpenAI-powered intelligent CLI tool for developers");
        
        var openAIService = serviceProvider.GetRequiredService<IOpenAIService>();
        
        var queryCommand = new QueryCommand(openAIService);
        rootCommand.Subcommands.Add(queryCommand);
        
        var versionCommand = new Command("version", "Show version information");
        versionCommand.SetAction((parseResult) =>
        {
            Console.WriteLine("qx version 0.1.0-alpha");
            Console.WriteLine("Copyright © 2025 Qx Development Team");
            return 0;
        });
        rootCommand.Subcommands.Add(versionCommand);
        
        return rootCommand;
    }
}