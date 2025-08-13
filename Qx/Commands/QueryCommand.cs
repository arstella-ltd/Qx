using System.CommandLine;
using Qx.Handlers;
using Qx.Services;

namespace Qx.Commands;

internal sealed class QueryCommand : Command
{
    public QueryCommand(IOpenAIService openAIService) : base("query", "Generate natural language response from user prompt")
    {
        var promptArgument = new Argument<string[]>("prompt")
        {
            Description = "The natural language prompt to send",
            Arity = ArgumentArity.OneOrMore
        };

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

        var maxTokensOption = new Option<int>("--max-tokens")
        {
            Description = "Maximum number of tokens in the response"
        };

        Arguments.Add(promptArgument);
        Options.Add(modelOption);
        Options.Add(outputOption);
        Options.Add(temperatureOption);
        Options.Add(maxTokensOption);

        var handler = new QueryCommandHandler(openAIService);

        this.SetAction((parseResult) =>
        {
            string[] prompt = parseResult.GetValue(promptArgument) ?? Array.Empty<string>();
            string model = parseResult.GetValue(modelOption) ?? "gpt-5-nano";
            string? output = parseResult.GetValue(outputOption);
            double temperature = parseResult.GetValue(temperatureOption);
            if (temperature == 0)
            {
                temperature = 1.0;
            }
            int maxTokens = parseResult.GetValue(maxTokensOption);
            if (maxTokens == 0)
            {
                maxTokens = 1000;
            }

            Task.Run(async () => await handler.HandleAsync(prompt, model, output, temperature, maxTokens).ConfigureAwait(false)).GetAwaiter().GetResult();
            return 0;
        });
    }
}
