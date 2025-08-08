using Microsoft.AspNetCore.Mvc.Testing;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using SearchEntities;

namespace Search.Tests;

public class PerformanceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PerformanceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Search_PerformanceTest_MeetsKPIRequirements()
    {
        // Arrange
        const int testIterations = 50;
        var latencies = new List<long>();
        var errors = 0;

        // Act
        for (int i = 0; i < testIterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var response = await _client.GetAsync($"/api/v1/search?q=test{i}&top=10");
                stopwatch.Stop();
                
                if (response.IsSuccessStatusCode)
                {
                    latencies.Add(stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    errors++;
                }
            }
            catch (Exception)
            {
                errors++;
                stopwatch.Stop();
            }
        }

        // Assert KPIs
        Assert.True(latencies.Count > 0, "No successful requests recorded");
        
        // Calculate percentiles
        latencies.Sort();
        var p50 = latencies[latencies.Count * 50 / 100];
        var p95 = latencies[latencies.Count * 95 / 100];
        var errorRate = (double)errors / testIterations;

        // PRD Requirements: P50 < 2s, P95 < 4s, error rate < 1%
        Assert.True(p50 < 2000, $"P50 latency {p50}ms exceeds 2000ms requirement");
        Assert.True(p95 < 4000, $"P95 latency {p95}ms exceeds 4000ms requirement");
        Assert.True(errorRate < 0.01, $"Error rate {errorRate:P2} exceeds 1% requirement");

        // Log results for visibility
        Console.WriteLine($"Performance Test Results:");
        Console.WriteLine($"  Total Requests: {testIterations}");
        Console.WriteLine($"  Successful: {latencies.Count}");
        Console.WriteLine($"  Errors: {errors}");
        Console.WriteLine($"  Error Rate: {errorRate:P2}");
        Console.WriteLine($"  P50 Latency: {p50}ms (target: <2000ms)");
        Console.WriteLine($"  P95 Latency: {p95}ms (target: <4000ms)");
        Console.WriteLine($"  Average Latency: {latencies.Average():F2}ms");
    }

    [Fact]
    public async Task Search_ConcurrentLoad_HandlesMultipleRequests()
    {
        // Arrange
        const int concurrentUsers = 10;
        const int requestsPerUser = 5;
        var allTasks = new List<Task<(bool Success, long Latency)>>();

        // Act
        for (int user = 0; user < concurrentUsers; user++)
        {
            for (int request = 0; request < requestsPerUser; request++)
            {
                var userRequest = user;
                var requestNumber = request;
                
                allTasks.Add(Task.Run(async () =>
                {
                    var stopwatch = Stopwatch.StartNew();
                    try
                    {
                        var response = await _client.GetAsync($"/api/v1/search?q=user{userRequest}query{requestNumber}");
                        stopwatch.Stop();
                        return (response.IsSuccessStatusCode, stopwatch.ElapsedMilliseconds);
                    }
                    catch
                    {
                        stopwatch.Stop();
                        return (false, stopwatch.ElapsedMilliseconds);
                    }
                }));
            }
        }

        var results = await Task.WhenAll(allTasks);

        // Assert
        var successfulRequests = results.Count(r => r.Success);
        var totalRequests = concurrentUsers * requestsPerUser;
        var successRate = (double)successfulRequests / totalRequests;
        var averageLatency = results.Where(r => r.Success).Average(r => r.Latency);

        Assert.True(successRate > 0.99, $"Success rate {successRate:P2} below 99%");
        Assert.True(averageLatency < 2000, $"Average latency {averageLatency:F2}ms exceeds 2000ms under load");

        Console.WriteLine($"Concurrent Load Test Results:");
        Console.WriteLine($"  Concurrent Users: {concurrentUsers}");
        Console.WriteLine($"  Requests per User: {requestsPerUser}");
        Console.WriteLine($"  Total Requests: {totalRequests}");
        Console.WriteLine($"  Successful: {successfulRequests}");
        Console.WriteLine($"  Success Rate: {successRate:P2}");
        Console.WriteLine($"  Average Latency: {averageLatency:F2}ms");
    }
}