using System.Text.Json.Serialization;

namespace SearchEntities;

/// <summary>
/// Represents information about a source page that contributed to search results
/// </summary>
public class SourcePageInfo
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("excerpt")]
    public string Excerpt { get; set; } = string.Empty;

    [JsonPropertyName("relevanceScore")]
    public float RelevanceScore { get; set; }

    [JsonPropertyName("indexedAt")]
    public DateTime IndexedAt { get; set; }

    [JsonPropertyName("chunkIndex")]
    public int ChunkIndex { get; set; }
}

[JsonSerializable(typeof(SourcePageInfo))]
public sealed partial class SourcePageInfoSerializerContext : JsonSerializerContext
{
}