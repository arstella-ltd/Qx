using System.Text.Json.Serialization;

namespace Qx.Services;

/// <summary>
/// Response details for verbose output
/// </summary>
internal sealed class ResponseDetails
{
    public string Model { get; set; } = string.Empty;
    public RequestOptions RequestOptions { get; set; } = new();
    public ResponseMetadata ResponseMetadata { get; set; } = new();
}

internal sealed class RequestOptions
{
    public double Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public bool WebSearchEnabled { get; set; }
}

internal sealed class ResponseMetadata
{
    public int? OutputItemsCount { get; set; }
    public int? ContentCount { get; set; }
    public string? FinishReason { get; set; }
    public UsageInfo? Usage { get; set; }
    public List<ItemInfo>? Items { get; set; }
}

internal sealed class UsageInfo
{
    public int PromptTokens { get; set; }
    public int CompletionTokens { get; set; }
    public int TotalTokens { get; set; }
}

internal sealed class ItemInfo
{
    public string Type { get; set; } = string.Empty;
    public ContentInfo? Content { get; set; }
    public string? WebSearchStatus { get; set; }
    public string? FunctionName { get; set; }
}

internal sealed class ContentInfo
{
    public int ContentCount { get; set; }
    public List<string?>? TextPreviews { get; set; }
}

/// <summary>
/// JSON serialization context for AOT compatibility
/// </summary>
[JsonSerializable(typeof(ResponseDetails))]
[JsonSerializable(typeof(RequestOptions))]
[JsonSerializable(typeof(ResponseMetadata))]
[JsonSerializable(typeof(UsageInfo))]
[JsonSerializable(typeof(ItemInfo))]
[JsonSerializable(typeof(ContentInfo))]
[JsonSourceGenerationOptions(WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class ResponseDetailsJsonContext : JsonSerializerContext
{
}
