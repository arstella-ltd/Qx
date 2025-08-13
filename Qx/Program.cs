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
        // Check if only help, version, or license is requested (no actual query)
        bool isHelpOrVersionOrLicense = args.Length == 0 ||
                              (args.Length > 0 && (args[0] == "--help" || args[0] == "-h" ||
                               args[0] == "--version" || args[0] == "version" ||
                               args[0] == "--license"));

        string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        // For actual queries, require API key
        if (!isHelpOrVersionOrLicense && string.IsNullOrEmpty(apiKey))
        {
            Console.Error.WriteLine("Error: OPENAI_API_KEY environment variable is not set");
            return 3;
        }

        // Set up services
        ServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IOpenAIService>(new OpenAIService(apiKey ?? "dummy-key-for-help"));
        serviceCollection.AddSingleton<ILicenseHelper, LicenseHelper>();

        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        RootCommand rootCommand = CommandRegistry.CreateRootCommand(serviceProvider);

        return rootCommand.Parse(args).Invoke();
    }
}
