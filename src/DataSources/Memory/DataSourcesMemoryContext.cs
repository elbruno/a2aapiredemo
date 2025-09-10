using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using DataSources.Models;
using SearchEntities;
using DataSources.Services;

namespace DataSources.Memory;

/// <summary>
/// In-memory vector store context for web page content
/// </summary>
public class DataSourcesMemoryContext
{
    private readonly ILogger _logger;
    private readonly IChatClient? _chatClient;
    private readonly IEmbeddingGenerator<string, Embedding<float>>? _embeddingGenerator;
    private readonly WebCrawlerService _webCrawlerService;
    private Dictionary<int, WebPageContent>? _webPagesCollection;
    private bool _isMemoryInitialized = false;
    private int _nextId = 1;

    public DataSourcesMemoryContext(
        ILogger logger, 
        IChatClient? chatClient, 
        IEmbeddingGenerator<string, Embedding<float>>? embeddingGenerator,
        WebCrawlerService webCrawlerService)
    {
        _logger = logger;
        _chatClient = chatClient;
        _embeddingGenerator = embeddingGenerator;
        _webCrawlerService = webCrawlerService;

        _logger.LogInformation("DataSources Memory context created");
        _logger.LogInformation($"Chat Client is null: {_chatClient is null}");
        _logger.LogInformation($"Embedding Generator is null: {_embeddingGenerator is null}");
    }

    /// <summary>
    /// Initialize the memory context and vector store
    /// </summary>
    public async Task<bool> InitMemoryContextAsync()
    {
        try
        {
            if (_embeddingGenerator == null)
            {
                _logger.LogError("Embedding generator is null, cannot initialize memory context");
                return false;
            }

            _logger.LogInformation("Initializing DataSources memory context");

            // Create in-memory dictionary for web pages (simplified approach)
            _webPagesCollection = new Dictionary<int, WebPageContent>();

            _isMemoryInitialized = true;
            _logger.LogInformation("DataSources memory context initialized successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing DataSources memory context");
            return false;
        }
    }

    /// <summary>
    /// Index URLs by crawling and storing their content in the vector store
    /// </summary>
    public async Task<List<IndexedUrl>> IndexUrlsAsync(List<string> urls)
    {
        if (!_isMemoryInitialized)
        {
            await InitMemoryContextAsync();
        }

        if (_webPagesCollection == null || _embeddingGenerator == null)
        {
            _logger.LogError("Memory context not properly initialized");
            return new List<IndexedUrl>();
        }

        var results = new List<IndexedUrl>();

        foreach (var url in urls)
        {
            try
            {
                _logger.LogInformation("Processing URL: {Url}", url);

                // Clear existing content for this URL
                ClearUrlContent(url);

                // Crawl the page
                var pageInfo = await _webCrawlerService.CrawlPageAsync(url);
                if (pageInfo == null)
                {
                    results.Add(new IndexedUrl
                    {
                        Url = url,
                        Title = "Failed to crawl",
                        ChunkCount = 0,
                        IndexedAt = DateTime.UtcNow,
                        Success = false,
                        ErrorMessage = "Failed to crawl page"
                    });
                    continue;
                }

                // Chunk the content
                var chunks = _webCrawlerService.ChunkText(pageInfo.Content);
                
                if (chunks.Count == 0)
                {
                    results.Add(new IndexedUrl
                    {
                        Url = url,
                        Title = pageInfo.Title,
                        ChunkCount = 0,
                        IndexedAt = DateTime.UtcNow,
                        Success = false,
                        ErrorMessage = "No content to index"
                    });
                    continue;
                }

                // Generate embeddings and store chunks
                int chunkIndex = 0;
                foreach (var chunk in chunks)
                {
                    try
                    {
                        var embedding = await _embeddingGenerator.GenerateVectorAsync(chunk);
                        
                        var webPageContent = new WebPageContent
                        {
                            Id = _nextId++,
                            Url = url,
                            Title = pageInfo.Title,
                            Content = pageInfo.Content,
                            ChunkContent = chunk,
                            IndexedAt = DateTime.UtcNow,
                            ChunkIndex = chunkIndex++,
                            Vector = embedding.ToArray()
                        };

                        _webPagesCollection[webPageContent.Id] = webPageContent;
                        _logger.LogInformation("Indexed chunk {ChunkIndex} for URL: {Url}", chunkIndex - 1, url);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error indexing chunk {ChunkIndex} for URL: {Url}", chunkIndex, url);
                    }
                }

                results.Add(new IndexedUrl
                {
                    Url = url,
                    Title = pageInfo.Title,
                    ChunkCount = chunks.Count,
                    IndexedAt = DateTime.UtcNow,
                    Success = true
                });

                _logger.LogInformation("Successfully indexed URL {Url} with {ChunkCount} chunks", url, chunks.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing URL: {Url}", url);
                results.Add(new IndexedUrl
                {
                    Url = url,
                    Title = "Error",
                    ChunkCount = 0,
                    IndexedAt = DateTime.UtcNow,
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        return results;
    }

    /// <summary>
    /// Search for web page content using semantic search
    /// </summary>
    public async Task<SearchResponse> SearchAsync(string query, int maxResults = 5)
    {
        var response = new SearchResponse { Products = new List<DataEntities.Product>() };

        try
        {
            if (!_isMemoryInitialized || _webPagesCollection == null || _embeddingGenerator == null)
            {
                response.Response = "DataSources memory context not initialized";
                return response;
            }

            _logger.LogInformation("Searching web pages for: {Query}", query);

            if (_webPagesCollection.Count == 0)
            {
                response.Response = "No web pages have been indexed yet. Please add some URLs first.";
                return response;
            }

            // Generate embedding for the query
            var queryEmbedding = await _embeddingGenerator.GenerateVectorAsync(query);

            // Simple similarity search using dictionary (simplified approach)
            var searchResults = _webPagesCollection.Values
                .Select(page => new
                {
                    Page = page,
                    Similarity = CalculateCosineSimilarity(queryEmbedding.ToArray(), page.Vector.ToArray())
                })
                .OrderByDescending(x => x.Similarity)
                .Take(maxResults)
                .Where(x => x.Similarity > 0.1) // Basic similarity threshold
                .ToList();

            var relevantContent = new List<string>();
            foreach (var result in searchResults)
            {
                relevantContent.Add($"From {result.Page.Title} ({result.Page.Url}): {result.Page.ChunkContent}");
            }

            if (relevantContent.Count == 0)
            {
                response.Response = "No relevant web page content found for your query.";
                return response;
            }

            // Generate response using chat client if available
            if (_chatClient != null)
            {
                var context = string.Join("\n\n", relevantContent);
                var systemPrompt = @"You are a helpful assistant that answers questions based on web page content. 
Use the provided context from web pages to answer the user's question. 
If the context doesn't contain relevant information, say so clearly.
Always mention the source URLs when referencing information.";

                var messages = new List<ChatMessage>
                {
                    new(ChatRole.System, systemPrompt),
                    new(ChatRole.User, $"Context from web pages:\n{context}\n\nUser question: {query}")
                };

                var chatResponse = await _chatClient.GetResponseAsync(messages);
                response.Response = chatResponse.Text ?? "Unable to generate response";
            }
            else
            {
                // Fallback response without chat client
                response.Response = $"Found {relevantContent.Count} relevant web page sections:\n\n" + 
                                  string.Join("\n\n", relevantContent.Take(3));
            }

            _logger.LogInformation("Search completed, found {Count} results", relevantContent.Count);
        }
        catch (Exception ex)
        {
            response.Response = $"Error during search: {ex.Message}";
            _logger.LogError(ex, "Error during web page search");
        }

        return response;
    }

    /// <summary>
    /// Clear all content for a specific URL
    /// </summary>
    private void ClearUrlContent(string url)
    {
        if (_webPagesCollection == null) return;

        try
        {
            var keysToRemove = _webPagesCollection
                .Where(kvp => kvp.Value.Url == url)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                _webPagesCollection.Remove(key);
            }

            _logger.LogInformation("Cleared {Count} existing records for URL: {Url}", keysToRemove.Count, url);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error clearing existing content for URL: {Url}", url);
        }
    }

    /// <summary>
    /// Get all indexed URLs
    /// </summary>
    public async Task<List<IndexedUrl>> GetIndexedUrlsAsync()
    {
        if (!_isMemoryInitialized || _webPagesCollection == null)
        {
            return new List<IndexedUrl>();
        }

        try
        {
            var urlsDict = new Dictionary<string, IndexedUrl>();

            foreach (var pageContent in _webPagesCollection.Values)
            {
                var url = pageContent.Url;
                if (!urlsDict.ContainsKey(url))
                {
                    urlsDict[url] = new IndexedUrl
                    {
                        Url = url,
                        Title = pageContent.Title,
                        ChunkCount = 0,
                        IndexedAt = pageContent.IndexedAt,
                        Success = true
                    };
                }
                urlsDict[url].ChunkCount++;
            }

            await Task.CompletedTask; // Make async for consistency
            return urlsDict.Values.OrderByDescending(u => u.IndexedAt).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting indexed URLs");
            return new List<IndexedUrl>();
        }
    }

    /// <summary>
    /// Calculate cosine similarity between two vectors
    /// </summary>
    private static float CalculateCosineSimilarity(float[] vector1, float[] vector2)
    {
        if (vector1.Length != vector2.Length)
            return 0;

        float dotProduct = 0;
        float magnitude1 = 0;
        float magnitude2 = 0;

        for (int i = 0; i < vector1.Length; i++)
        {
            dotProduct += vector1[i] * vector2[i];
            magnitude1 += vector1[i] * vector1[i];
            magnitude2 += vector2[i] * vector2[i];
        }

        if (magnitude1 == 0 || magnitude2 == 0)
            return 0;

        return dotProduct / (float)(Math.Sqrt(magnitude1) * Math.Sqrt(magnitude2));
    }
}