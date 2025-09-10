using System.Text.Json.Serialization;

namespace SearchEntities;

public class DataSourcesSearchResponse
{
    public DataSourcesSearchResponse()
    {
        Products = new List<DataEntities.Product>();
        Response = string.Empty;
    }

    [JsonPropertyName("id")]
    public string? Response { get; set; }

    [JsonPropertyName("products")]
    public List<DataEntities.Product>? Products { get; set; }

}


[JsonSerializable(typeof(DataSourcesSearchResponse))]
public sealed partial class DataSourcesSearchResponseSerializerContext : JsonSerializerContext
{
}