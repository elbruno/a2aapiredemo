using System.ComponentModel.DataAnnotations;

namespace DataSources.Models;

/// <summary>
/// Request model for indexing URLs
/// </summary>
public class IndexUrlsRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one URL is required")]
    [MaxLength(5, ErrorMessage = "Maximum 5 URLs allowed")]
    public required List<string> Urls { get; set; }
}

/// <summary>
/// Response model for indexing URLs
/// </summary>
public class IndexUrlsResponse
{
    public required List<IndexedUrl> IndexedUrls { get; set; }
    public required List<string> Errors { get; set; }
}

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