using System.Text.Json.Serialization;

namespace SearchEntities;

/// <summary>
/// Response entity for DataSources semantic search containing results and source page information
/// </summary>
public class DataSourcesSearchResponse
{
    public DataSourcesSearchResponse()
    {
        Response = string.Empty;
        SourcePages = new List<SourcePageInfo>();
    }

    /// <summary>
    /// The main response text/answer generated from the search results
    /// </summary>
    [JsonPropertyName("response")]
    public string Response { get; set; }

    /// <summary>
    /// Collection of source pages that contributed to the search results
    /// </summary>
    [JsonPropertyName("sourcePages")]
    public List<SourcePageInfo> SourcePages { get; set; }

    /// <summary>
    /// Number of relevant sources found
    /// </summary>
    [JsonPropertyName("sourceCount")]
    public int SourceCount => SourcePages?.Count ?? 0;

    /// <summary>
    /// Indicates if the search found relevant content
    /// </summary>
    [JsonPropertyName("hasResults")]
    public bool HasResults => !string.IsNullOrEmpty(Response) && SourcePages?.Any() == true;
}

[JsonSerializable(typeof(DataSourcesSearchResponse))]
public sealed partial class DataSourcesSearchResponseSerializerContext : JsonSerializerContext
{
}