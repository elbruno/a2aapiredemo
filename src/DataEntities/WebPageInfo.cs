namespace DataEntities;

/// <summary>
/// Information extracted from a web page
/// </summary>
public class WebPageInfo
{
    public required string Url { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime CrawledAt { get; set; }
}