namespace Qx.Tests.Helpers;

/// <summary>
/// Constants used across test classes
/// </summary>
internal static class TestConstants
{
    /// <summary>
    /// Test API key for mocking
    /// </summary>
    public const string TestApiKey = "test-api-key-12345";

    /// <summary>
    /// Test queries for various scenarios
    /// </summary>
    internal static class Queries
    {
        public const string Simple = "What is 2+2?";
        public const string Complex = "Explain quantum computing in simple terms";
        public const string WithSpecialChars = "What's the difference between \"interface\" and 'abstract class'?";
        public const string Multiline = @"Can you help me with:
1. Understanding async/await
2. Best practices for error handling
3. Performance optimization";
    }

    /// <summary>
    /// Expected exit codes
    /// </summary>
    internal static class ExitCodes
    {
        public const int Success = 0;
        public const int GeneralError = 1;
        public const int ApiConnectionError = 2;
        public const int AuthenticationError = 3;
        public const int Timeout = 124;
    }

    /// <summary>
    /// Timeout values for tests
    /// </summary>
    internal static class Timeouts
    {
        public const int Brief = 1;
        public const int Default = 60;
        public const int Extended = 300;
    }

    /// <summary>
    /// Effort levels
    /// </summary>
    internal static class EffortLevels
    {
        public const string Low = "low";
        public const string Medium = "medium";
        public const string High = "high";
    }

    /// <summary>
    /// Context sizes
    /// </summary>
    internal static class ContextSizes
    {
        public const string Low = "low";
        public const string Medium = "medium";
        public const string High = "high";
    }

    /// <summary>
    /// Sample responses for mocking
    /// </summary>
    internal static class SampleResponses
    {
        public const string Simple = "The answer is 4.";
        public const string WithCode = @"Here's an example:

```csharp
public class Example
{
    public void Method() 
    {
        Console.WriteLine(""Hello"");
    }
}
```

This demonstrates a simple C# class.";
        
        public const string Error = "An error occurred while processing your request.";
    }
}