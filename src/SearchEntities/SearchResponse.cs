using System.Text.Json.Serialization;

namespace SearchEntities;

public class SearchRequest
{
    [JsonPropertyName("q")]
    public string Query { get; set; } = string.Empty;

    [JsonPropertyName("top")]
    public int Top { get; set; } = 10;

    [JsonPropertyName("skip")]
    public int Skip { get; set; } = 0;
}

public class SearchResult
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("snippet")]
    public string Snippet { get; set; } = string.Empty;

    [JsonPropertyName("score")]
    public double Score { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class SearchResponse
{
    public SearchResponse()
    {
        Results = new List<SearchResult>();
        Products = new List<DataEntities.Product>();
        Response = string.Empty;
    }

    [JsonPropertyName("query")]
    public string Query { get; set; } = string.Empty;

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("results")]
    public List<SearchResult> Results { get; set; }

    // Legacy properties for backward compatibility with existing Store UI
    [JsonPropertyName("id")]
    public string? Response { get; set; }

    [JsonPropertyName("products")]
    public List<DataEntities.Product>? Products { get; set; }
}

public class SearchErrorResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("correlationId")]
    public string CorrelationId { get; set; } = string.Empty;
}

public class ReindexRequest
{
    [JsonPropertyName("siteBaseUrl")]
    public string? SiteBaseUrl { get; set; }

    [JsonPropertyName("force")]
    public bool Force { get; set; } = false;
}

public class ReindexResponse
{
    [JsonPropertyName("operationId")]
    public string OperationId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("startedAt")]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
}


[JsonSerializable(typeof(SearchRequest))]
[JsonSerializable(typeof(SearchResponse))]
[JsonSerializable(typeof(SearchErrorResponse))]
[JsonSerializable(typeof(ReindexRequest))]
[JsonSerializable(typeof(ReindexResponse))]
public sealed partial class SearchSerializerContext : JsonSerializerContext
{
}