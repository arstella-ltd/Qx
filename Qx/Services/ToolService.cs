using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
#pragma warning disable OPENAI001, CA1031, CA5394, CA1307, IDE0008, CA2000
using OpenAI.Responses;
#pragma warning restore OPENAI001, CA1031, CA5394, CA1307, IDE0008, CA2000

namespace Qx.Services;

/// <summary>
/// Service for handling tool/function calling
/// </summary>
internal sealed class ToolService
{
    private readonly Dictionary<string, Func<JsonDocument, string>> _functionHandlers;
#pragma warning disable OPENAI001
    private readonly List<ResponseTool> _availableTools;
#pragma warning restore OPENAI001

    public ToolService()
    {
        _functionHandlers = new Dictionary<string, Func<JsonDocument, string>>(StringComparer.OrdinalIgnoreCase);
#pragma warning disable OPENAI001
        _availableTools = new List<ResponseTool>();
#pragma warning restore OPENAI001
        
        // Register built-in functions
        RegisterBuiltInFunctions();
    }

    /// <summary>
    /// Get all available tools for the Response API
    /// </summary>
#pragma warning disable OPENAI001
    public IReadOnlyList<ResponseTool> GetAvailableTools() => _availableTools.AsReadOnly();
#pragma warning restore OPENAI001

    /// <summary>
    /// Execute a function call
    /// </summary>
    public string ExecuteFunction(string functionName, BinaryData? arguments)
    {
        if (!_functionHandlers.TryGetValue(functionName, out var handler))
        {
            return $"Error: Unknown function '{functionName}'";
        }

        try
        {
            if (arguments == null || arguments.ToString() == "{}")
            {
                // No arguments provided
                return handler(JsonDocument.Parse("{}"));
            }

            using JsonDocument doc = JsonDocument.Parse(arguments.ToString());
            return handler(doc);
        }
        catch (JsonException ex)
        {
            return $"Error parsing arguments: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Error executing function: {ex.Message}";
        }
    }

    private void RegisterBuiltInFunctions()
    {
        // Register GetCurrentTime function
        RegisterFunction(
            "GetCurrentTime",
            "Get the current date and time in a specific timezone",
            """
            {
                "type": "object",
                "properties": {
                    "timezone": {
                        "type": "string",
                        "description": "The timezone (e.g., 'UTC', 'PST', 'JST'). Default is UTC."
                    }
                },
                "required": []
            }
            """,
            GetCurrentTime
        );

        // Register GetWeather function (mock implementation)
        RegisterFunction(
            "GetWeather",
            "Get the current weather for a location",
            """
            {
                "type": "object",
                "properties": {
                    "location": {
                        "type": "string",
                        "description": "The city and state/country, e.g., 'San Francisco, CA' or 'Tokyo, Japan'"
                    },
                    "unit": {
                        "type": "string",
                        "enum": ["celsius", "fahrenheit"],
                        "description": "The temperature unit to use"
                    }
                },
                "required": ["location"]
            }
            """,
            GetWeather
        );

        // Register CalculateExpression function
        RegisterFunction(
            "CalculateExpression",
            "Evaluate a mathematical expression",
            """
            {
                "type": "object",
                "properties": {
                    "expression": {
                        "type": "string",
                        "description": "The mathematical expression to evaluate (e.g., '2 + 2 * 3')"
                    }
                },
                "required": ["expression"]
            }
            """,
            CalculateExpression
        );
    }

    private void RegisterFunction(string name, string description, string parametersJson, Func<JsonDocument, string> handler)
    {
        _functionHandlers[name] = handler;
        
#pragma warning disable OPENAI001
        ResponseTool tool = ResponseTool.CreateFunctionTool(
            functionName: name,
            functionDescription: description,
            functionParameters: BinaryData.FromString(parametersJson),
            functionSchemaIsStrict: false
        );
#pragma warning restore OPENAI001
        
        _availableTools.Add(tool);
    }

    // Built-in function implementations
    private static string GetCurrentTime(JsonDocument args)
    {
        string timezone = "UTC";
        
        if (args.RootElement.TryGetProperty("timezone", out JsonElement tzElement))
        {
            timezone = tzElement.GetString() ?? "UTC";
        }

        try
        {
            DateTime now = timezone.ToUpperInvariant() switch
            {
                "PST" or "PT" => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, 
                    TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles")),
                "EST" or "ET" => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, 
                    TimeZoneInfo.FindSystemTimeZoneById("America/New_York")),
                "JST" => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, 
                    TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo")),
                "GMT" or "UTC" => DateTime.UtcNow,
                _ => DateTime.UtcNow
            };

            return $"Current time in {timezone}: {now:yyyy-MM-dd HH:mm:ss}";
        }
        catch
        {
            return $"Current time in UTC: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
        }
    }

    private static string GetWeather(JsonDocument args)
    {
        if (!args.RootElement.TryGetProperty("location", out JsonElement locationElement))
        {
            return "Error: Location is required";
        }

        string location = locationElement.GetString() ?? "Unknown";
        string unit = "celsius";
        
        if (args.RootElement.TryGetProperty("unit", out JsonElement unitElement))
        {
            unit = unitElement.GetString() ?? "celsius";
        }

        // Mock weather data (in a real implementation, this would call a weather API)
        Random rand = new Random(location.GetHashCode());
        int temp = rand.Next(15, 30);
        string[] conditions = { "Sunny", "Partly Cloudy", "Cloudy", "Light Rain", "Clear" };
        string condition = conditions[rand.Next(conditions.Length)];

        if (unit == "fahrenheit")
        {
            temp = (int)(temp * 9.0 / 5.0 + 32);
        }

        string unitSymbol = unit == "fahrenheit" ? "°F" : "°C";
        return $"Weather in {location}: {condition}, {temp}{unitSymbol}";
    }

    private static string CalculateExpression(JsonDocument args)
    {
        if (!args.RootElement.TryGetProperty("expression", out JsonElement exprElement))
        {
            return "Error: Expression is required";
        }

        string expression = exprElement.GetString() ?? "";
        
        try
        {
            // Simple expression evaluator (for basic operations)
            // In production, use a proper expression evaluator library
            double result = EvaluateSimpleExpression(expression);
            return $"Result: {result}";
        }
        catch (Exception ex)
        {
            return $"Error evaluating expression: {ex.Message}";
        }
    }

    private static double EvaluateSimpleExpression(string expression)
    {
        // This is a very basic implementation for demonstration
        // Only handles simple arithmetic expressions for AOT compatibility
        
        // Remove spaces
        expression = expression.Replace(" ", "", StringComparison.Ordinal);
        
        // For this demo, we'll just handle simple cases
        if (double.TryParse(expression, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
        {
            return value;
        }

        // Simple evaluation for basic operations (AOT-compatible)
        // This is limited but works for demo purposes
        try
        {
            // Handle simple addition
            if (expression.Contains('+'))
            {
                string[] parts = expression.Split('+');
                if (parts.Length == 2 && 
                    double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double left) &&
                    double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double right))
                {
                    return left + right;
                }
            }
            
            // Handle simple subtraction
            if (expression.Contains('-') && !expression.StartsWith('-'))
            {
                string[] parts = expression.Split('-');
                if (parts.Length == 2 && 
                    double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double left) &&
                    double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double right))
                {
                    return left - right;
                }
            }
            
            // Handle simple multiplication
            if (expression.Contains('*'))
            {
                string[] parts = expression.Split('*');
                if (parts.Length == 2 && 
                    double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double left) &&
                    double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double right))
                {
                    return left * right;
                }
            }
            
            // Handle simple division
            if (expression.Contains('/'))
            {
                string[] parts = expression.Split('/');
                if (parts.Length == 2 && 
                    double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double left) &&
                    double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double right))
                {
                    if (right == 0)
                    {
                        throw new DivideByZeroException("Cannot divide by zero");
                    }
                    return left / right;
                }
            }
        }
        catch
        {
            // If parsing fails, throw an exception
            throw new ArgumentException($"Unable to evaluate expression: {expression}");
        }
        
        throw new ArgumentException($"Unable to evaluate expression: {expression}");
    }
}

/// <summary>
/// JSON context for tool-related serialization
/// </summary>
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(List<string>))]
[JsonSourceGenerationOptions(WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class ToolJsonContext : JsonSerializerContext
{
}