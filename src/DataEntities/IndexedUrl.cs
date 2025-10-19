namespace DataEntities;

/// <summary>
/// Information about an indexed URL
/// </summary>
public class IndexedUrl
{
    public required string Url { get; set; }
    public required string Title { get; set; }
    public int ChunkCount { get; set; }
    public DateTime IndexedAt { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}