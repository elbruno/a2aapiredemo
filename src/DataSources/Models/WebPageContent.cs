using Microsoft.Extensions.VectorData;
using System.Text.Json.Serialization;

namespace DataSources.Models;

/// <summary>
/// Represents web page content stored in the vector database
/// </summary>
public class WebPageContent
{
    [VectorStoreKey]
    public int Id { get; set; }

    [VectorStoreData]
    public required string Url { get; set; }

    [VectorStoreData]
    public required string Title { get; set; }

    [VectorStoreData]
    public required string Content { get; set; }

    [VectorStoreData]
    public required string ChunkContent { get; set; }

    [VectorStoreData]
    public DateTime IndexedAt { get; set; }

    [VectorStoreData]
    public int ChunkIndex { get; set; }

    [VectorStoreVector(384)]  // Using 384 dimensions like the existing ProductVector
    public ReadOnlyMemory<float> Vector { get; set; }
}