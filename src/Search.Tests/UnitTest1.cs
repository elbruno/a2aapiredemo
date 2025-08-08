using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using SearchEntities;

namespace Search.Tests;

public class SearchApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SearchApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Search_WithValidQuery_ReturnsResults()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/search?q=running");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var searchResponse = JsonSerializer.Deserialize<SearchResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(searchResponse);
        Assert.Equal("running", searchResponse.Query);
        Assert.True(searchResponse.Count >= 0);
        Assert.NotNull(searchResponse.Results);
    }

    [Fact]
    public async Task Search_WithEmptyQuery_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/search?q=");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Search_WithNoQuery_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/search");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Search_WithInvalidTop_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/search?q=test&top=100");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Search_WithValidParameters_ReturnsCorrectCount()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/search?q=shoes&top=2&skip=0");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var searchResponse = JsonSerializer.Deserialize<SearchResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(searchResponse);
        Assert.True(searchResponse.Results.Count <= 2);
    }

    [Fact]
    public async Task Reindex_ReturnsAccepted()
    {
        // Arrange
        var requestBody = JsonSerializer.Serialize(new ReindexRequest { SiteBaseUrl = "https://test.com", Force = false });
        var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/search/reindex", content);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var reindexResponse = JsonSerializer.Deserialize<ReindexResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(reindexResponse);
        Assert.NotEmpty(reindexResponse.OperationId);
        Assert.Equal("started", reindexResponse.Status);
    }
}
