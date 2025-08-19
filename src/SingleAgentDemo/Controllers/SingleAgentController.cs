using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using SingleAgentDemo.Models;

namespace SingleAgentDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SingleAgentController : ControllerBase
{
    private readonly ILogger<SingleAgentController> _logger;
    private readonly Kernel _kernel;
    private readonly IHttpClientFactory _httpClientFactory;

    public SingleAgentController(
        ILogger<SingleAgentController> logger,
        Kernel kernel,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _kernel = kernel;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<SingleAgentAnalysisResponse>> AnalyzeAsync(
        [FromForm] IFormFile image,
        [FromForm] string prompt,
        [FromForm] string customerId)
    {
        try
        {
            _logger.LogInformation("Starting analysis for customer {CustomerId}", customerId);

            // Step 1: Analyze the image
            var photoAnalysis = await AnalyzePhotoAsync(image, prompt);
            
            // Step 2: Get customer information
            var customerInfo = await GetCustomerInformationAsync(customerId);
            
            // Step 3: Use Semantic Kernel to reason about tools needed
            var reasoning = await GenerateToolReasoningAsync(photoAnalysis, customerInfo, prompt);
            
            // Step 4: Match tools and get inventory
            var toolMatch = await MatchToolsAsync(customerId, photoAnalysis.DetectedMaterials, prompt);
            
            // Step 5: Enrich with inventory information
            var enrichedTools = await EnrichWithInventoryAsync(toolMatch.MissingTools);

            var response = new SingleAgentAnalysisResponse
            {
                Analysis = photoAnalysis.Description,
                ReusableTools = toolMatch.ReusableTools,
                RecommendedTools = enrichedTools,
                Reasoning = reasoning
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing request for customer {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    private async Task<PhotoAnalysisResult> AnalyzePhotoAsync(IFormFile image, string prompt)
    {
        var client = _httpClientFactory.CreateClient("PhotoAnalysis");
        
        using var content = new MultipartFormDataContent();
        using var imageStream = image.OpenReadStream();
        using var streamContent = new StreamContent(imageStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
        
        content.Add(streamContent, "image", image.FileName);
        content.Add(new StringContent(prompt), "prompt");

        var response = await client.PostAsync("/api/mock/photo-analysis", content);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PhotoAnalysisResult>();
            return result ?? new PhotoAnalysisResult { Description = "Image analysis completed", DetectedMaterials = new[] { "paint", "wall" } };
        }

        // Fallback for demo purposes
        return new PhotoAnalysisResult 
        { 
            Description = $"Room analysis for prompt: {prompt}. Detected painted walls with preparation needed.",
            DetectedMaterials = new[] { "paint", "wall", "surface preparation" }
        };
    }

    private async Task<CustomerInformation> GetCustomerInformationAsync(string customerId)
    {
        var client = _httpClientFactory.CreateClient("CustomerInformation");
        
        var response = await client.GetAsync($"/api/mock/customer/{customerId}");
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<CustomerInformation>();
            return result ?? CreateFallbackCustomer(customerId);
        }

        return CreateFallbackCustomer(customerId);
    }

    private CustomerInformation CreateFallbackCustomer(string customerId)
    {
        return new CustomerInformation
        {
            Id = customerId,
            Name = $"Customer {customerId}",
            OwnedTools = new[] { "hammer", "screwdriver", "measuring tape" },
            Skills = new[] { "basic DIY", "painting" }
        };
    }

    private async Task<string> GenerateToolReasoningAsync(PhotoAnalysisResult photoAnalysis, CustomerInformation customer, string prompt)
    {
        try
        {
            var reasoningPrompt = $@"
Based on the following information, provide reasoning for tool recommendations:

Task: {prompt}
Image Analysis: {photoAnalysis.Description}
Detected Materials: {string.Join(", ", photoAnalysis.DetectedMaterials)}
Customer Tools: {string.Join(", ", customer.OwnedTools)}
Customer Skills: {string.Join(", ", customer.Skills)}

Provide clear reasoning for what tools are needed and why.";

            var response = await _kernel.InvokePromptAsync(reasoningPrompt);
            return response.GetValue<string>() ?? "Tool analysis completed based on image and customer profile.";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to generate AI reasoning, using fallback");
            return $"Based on the task '{prompt}' and the detected materials ({string.Join(", ", photoAnalysis.DetectedMaterials)}), specific tools will be recommended to complement your existing tools.";
        }
    }

    private async Task<ToolMatchResult> MatchToolsAsync(string customerId, string[] detectedMaterials, string prompt)
    {
        var client = _httpClientFactory.CreateClient("CustomerWork");
        
        var matchRequest = new
        {
            customerId,
            detectedMaterials,
            prompt
        };

        var response = await client.PostAsJsonAsync("/api/mock/customer-work/match", matchRequest);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ToolMatchResult>();
            return result ?? CreateFallbackToolMatch();
        }

        return CreateFallbackToolMatch();
    }

    private ToolMatchResult CreateFallbackToolMatch()
    {
        return new ToolMatchResult
        {
            ReusableTools = new[] { "measuring tape", "screwdriver" },
            MissingTools = new[]
            {
                new ToolRecommendation { Name = "Paint Roller", Sku = "PAINT-ROLLER-9IN", IsAvailable = true, Price = 12.99m, Description = "9-inch paint roller for smooth walls" },
                new ToolRecommendation { Name = "Paint Brush Set", Sku = "BRUSH-SET-3PC", IsAvailable = true, Price = 24.99m, Description = "3-piece brush set for detail work" },
                new ToolRecommendation { Name = "Drop Cloth", Sku = "DROP-CLOTH-9X12", IsAvailable = true, Price = 8.99m, Description = "Plastic drop cloth protection" }
            }
        };
    }

    private async Task<ToolRecommendation[]> EnrichWithInventoryAsync(ToolRecommendation[] tools)
    {
        var client = _httpClientFactory.CreateClient("ZavaInventory");
        
        var skus = tools.Select(t => t.Sku).ToArray();
        var response = await client.PostAsJsonAsync("/api/mock/inventory/search", new { skus });
        
        if (response.IsSuccessStatusCode)
        {
            var inventoryResults = await response.Content.ReadFromJsonAsync<ToolRecommendation[]>();
            return inventoryResults ?? tools;
        }

        return tools; // Return original tools if inventory service fails
    }
}