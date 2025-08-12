using Moq;
using OpenAI.Chat;

namespace Qx.Tests.Helpers;

/// <summary>
/// Factory for creating commonly used mock objects
/// </summary>
public static class MockFactory
{
    /// <summary>
    /// Create a mock environment with the specified variables
    /// </summary>
    public static IDisposable CreateMockEnvironment(Dictionary<string, string?> variables)
    {
        var originalValues = new Dictionary<string, string?>();
        
        foreach (var kvp in variables)
        {
            originalValues[kvp.Key] = Environment.GetEnvironmentVariable(kvp.Key);
            Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
        }
        
        return new EnvironmentRestorer(originalValues);
    }

    /// <summary>
    /// Create command line arguments from a string
    /// </summary>
    public static string[] CreateArgs(string commandLine)
    {
        if (string.IsNullOrWhiteSpace(commandLine))
        {
            return Array.Empty<string>();
        }
            
        // Simple argument parser - doesn't handle all edge cases
        var args = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;
        
        foreach (char c in commandLine)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ' ' && !inQuotes)
            {
                if (current.Length > 0)
                {
                    args.Add(current.ToString());
                    current.Clear();
                }
            }
            else
            {
                current.Append(c);
            }
        }
        
        if (current.Length > 0)
        {
            args.Add(current.ToString());
        }
        
        return args.ToArray();
    }

    /// <summary>
    /// Create a mock ChatClient for testing OpenAI integration
    /// </summary>
    public static Mock<ChatClient> CreateMockChatClient()
    {
        // Note: ChatClient is sealed, so we need to use an interface wrapper in production code
        // This is a placeholder showing the pattern
        throw new NotImplementedException("OpenAI client mocking requires interface wrapper");
    }

    /// <summary>
    /// Assert that two string arrays are equivalent
    /// </summary>
    public static void AssertArgsEqual(string[] expected, string[] actual)
    {
        actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    private sealed class EnvironmentRestorer : IDisposable
    {
        private readonly Dictionary<string, string?> _originalValues;

        public EnvironmentRestorer(Dictionary<string, string?> originalValues)
        {
            _originalValues = originalValues;
        }

        public void Dispose()
        {
            foreach (var kvp in _originalValues)
            {
                Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
            }
        }
    }
}