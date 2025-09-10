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
/// <param name="IndexedUrls"></param>
/// <param name="Errors"></param>
public record IndexUrlsResponse(List<IndexedUrl> IndexedUrls, List<string> Errors);
