namespace SemanticSearchFunction.Models;

public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public int? Top { get; set; }
}

public class SearchResponse
{
    public List<SearchResult> Results { get; set; } = new();
    public string TraceId { get; set; } = string.Empty;
}

public class SearchResult
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public double Score { get; set; }
    public string Snippet { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}