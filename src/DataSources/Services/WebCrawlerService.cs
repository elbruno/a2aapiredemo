using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;

namespace DataSources.Services;

/// <summary>
/// Service for crawling and extracting content from web pages
/// </summary>
public class WebCrawlerService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebCrawlerService> _logger;

    public WebCrawlerService(HttpClient httpClient, ILogger<WebCrawlerService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // Configure HttpClient
        _httpClient.DefaultRequestHeaders.Add("User-Agent", 
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
    }

    /// <summary>
    /// Crawls a web page and extracts its content
    /// </summary>
    public async Task<WebPageInfo?> CrawlPageAsync(string url)
    {
        try
        {
            _logger.LogInformation("Crawling URL: {Url}", url);

            // Validate URL
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || 
                (uri.Scheme != "http" && uri.Scheme != "https"))
            {
                _logger.LogWarning("Invalid URL format: {Url}", url);
                return null;
            }

            // Fetch the page
            var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            
            // Parse HTML
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Extract title
            var title = ExtractTitle(doc) ?? "Untitled";

            // Extract main content
            var content = ExtractMainContent(doc);

            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning("No content extracted from URL: {Url}", url);
                return null;
            }

            _logger.LogInformation("Successfully crawled URL: {Url}, Content length: {Length}", url, content.Length);

            return new WebPageInfo
            {
                Url = url,
                Title = title,
                Content = content,
                CrawledAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crawling URL: {Url}", url);
            return null;
        }
    }

    /// <summary>
    /// Chunks text into smaller pieces for vector embedding
    /// </summary>
    public List<string> ChunkText(string text, int maxChunkSize = 1000, int overlap = 200)
    {
        var chunks = new List<string>();
        
        if (string.IsNullOrWhiteSpace(text))
            return chunks;

        // Simple sentence-aware chunking
        var sentences = SplitIntoSentences(text);
        var currentChunk = new StringBuilder();
        
        foreach (var sentence in sentences)
        {
            // If adding this sentence would exceed the limit, start a new chunk
            if (currentChunk.Length + sentence.Length > maxChunkSize && currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString().Trim());
                
                // Start new chunk with overlap
                var overlapText = GetOverlapText(currentChunk.ToString(), overlap);
                currentChunk.Clear();
                if (!string.IsNullOrEmpty(overlapText))
                {
                    currentChunk.Append(overlapText);
                    currentChunk.Append(" ");
                }
            }
            
            currentChunk.Append(sentence);
            currentChunk.Append(" ");
        }
        
        // Add the last chunk if it has content
        if (currentChunk.Length > 0)
        {
            chunks.Add(currentChunk.ToString().Trim());
        }

        return chunks;
    }

    private string? ExtractTitle(HtmlDocument doc)
    {
        // Try different title sources
        var titleNode = doc.DocumentNode.SelectSingleNode("//title") ??
                       doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']") ??
                       doc.DocumentNode.SelectSingleNode("//h1");

        return titleNode?.InnerText?.Trim();
    }

    private string ExtractMainContent(HtmlDocument doc)
    {
        // Remove script and style elements
        var elementsToRemove = doc.DocumentNode.SelectNodes("//script | //style | //nav | //header | //footer | //aside | //form");
        if (elementsToRemove != null)
        {
            foreach (var element in elementsToRemove)
                element.Remove();
        }

        // Try to find main content areas
        var mainContent = doc.DocumentNode.SelectSingleNode("//main") ??
                         doc.DocumentNode.SelectSingleNode("//article") ??
                         doc.DocumentNode.SelectSingleNode("//div[@class*='content']") ??
                         doc.DocumentNode.SelectSingleNode("//div[@id*='content']") ??
                         doc.DocumentNode.SelectSingleNode("//body");

        var text = mainContent?.InnerText ?? "";
        
        // Clean up whitespace
        text = Regex.Replace(text, @"\s+", " ");
        text = text.Trim();

        return text;
    }

    private List<string> SplitIntoSentences(string text)
    {
        // Simple sentence splitting on common punctuation
        var sentences = Regex.Split(text, @"(?<=[.!?])\s+")
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        return sentences;
    }

    private string GetOverlapText(string text, int overlapSize)
    {
        if (text.Length <= overlapSize)
            return text;

        // Try to find a good break point near the overlap size
        var overlapText = text.Substring(Math.Max(0, text.Length - overlapSize));
        var spaceIndex = overlapText.IndexOf(' ');
        
        return spaceIndex > 0 ? overlapText.Substring(spaceIndex + 1) : overlapText;
    }
}

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