using System.Text.Json.Serialization;

namespace SearchEntities;

public class ProductSearchResponse
{
    public ProductSearchResponse()
    {
        Products = new List<DataEntities.Product>();
        Response = string.Empty;
    }

    [JsonPropertyName("id")]
    public string? Response { get; set; }

    [JsonPropertyName("products")]
    public List<DataEntities.Product>? Products { get; set; }

}


[JsonSerializable(typeof(ProductSearchResponse))]
public sealed partial class ProductSearchResponseSerializerContext : JsonSerializerContext
{
}