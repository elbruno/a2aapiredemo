using Microsoft.Extensions.Logging;
using Search.Services;
using Moq;

namespace Search.Tests;

public class MockNlWebClientTests
{
    private readonly Mock<ILogger<MockNlWebClient>> _loggerMock;
    private readonly Mock<HttpClient> _httpClientMock;
    private readonly MockNlWebClient _client;

    public MockNlWebClientTests()
    {
        _loggerMock = new Mock<ILogger<MockNlWebClient>>();
        _httpClientMock = new Mock<HttpClient>();
        _client = new MockNlWebClient(_loggerMock.Object, _httpClientMock.Object);
    }

    [Fact]
    public async Task QueryAsync_WithValidQuery_ReturnsResults()
    {
        // Arrange
        var query = "running shoes";
        var top = 10;
        var skip = 0;

        // Act
        var result = await _client.QueryAsync(query, top, skip);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(query, result.Query);
        Assert.True(result.Count > 0);
        Assert.NotNull(result.Results);
        Assert.True(result.Results.Count <= top);
    }

    [Fact]
    public async Task QueryAsync_WithEmptyQuery_ReturnsAllResults()
    {
        // Arrange
        var query = "";
        var top = 5;
        var skip = 0;

        // Act
        var result = await _client.QueryAsync(query, top, skip);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(query, result.Query);
        Assert.True(result.Count > 0);
        Assert.NotNull(result.Results);
    }

    [Fact]
    public async Task QueryAsync_WithSkip_ReturnsCorrectResults()
    {
        // Arrange
        var query = "";
        var top = 2;
        var skip = 2;

        // Act
        var result = await _client.QueryAsync(query, top, skip);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Results.Count <= top);
    }

    [Fact]
    public async Task ReindexAsync_ReturnsValidResponse()
    {
        // Arrange
        var siteBaseUrl = "https://test.com";
        var force = true;

        // Act
        var result = await _client.ReindexAsync(siteBaseUrl, force);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.OperationId);
        Assert.Equal("started", result.Status);
        Assert.Contains(siteBaseUrl, result.Message);
        Assert.True(result.StartedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task QueryAsync_PerformanceTest_CompletesWithinTimeLimit()
    {
        // Arrange
        var query = "test query";
        var startTime = DateTime.UtcNow;

        // Act
        var result = await _client.QueryAsync(query);
        var endTime = DateTime.UtcNow;

        // Assert
        var duration = endTime - startTime;
        Assert.True(duration.TotalMilliseconds < 2000, $"Query took {duration.TotalMilliseconds}ms, expected < 2000ms");
        Assert.NotNull(result);
    }
}