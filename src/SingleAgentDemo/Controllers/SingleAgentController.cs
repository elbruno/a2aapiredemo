using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
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

            // Create a single agent that orchestrates the entire process
            var agent = CreateZavaAgentAssistant();
            
            // Step 1: Analyze the image
            var photoAnalysis = await AnalyzePhotoAsync(image, prompt);
            
            // Step 2: Get customer information
            var customerInfo = await GetCustomerInformationAsync(customerId);
            
            // Step 3: Use Single Semantic Kernel Agent to reason about tools needed
            var reasoning = await GenerateToolReasoningWithAgentAsync(agent, photoAnalysis, customerInfo, prompt);
            
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

    private ChatCompletionAgent CreateZavaAgentAssistant()
    {
        return new ChatCompletionAgent()
        {
            Name = "ZavaAssistant",
            Instructions = @"
You are Zava, an expert DIY and home improvement assistant. Your role is to:

1. Analyze customer projects and provide detailed tool recommendations
2. Consider the customer's existing tools and skill level
3. Provide clear, practical reasoning for each recommendation
4. Prioritize safety in all recommendations
5. Be encouraging while being realistic about project complexity
6. Offer specific tips based on the customer's skill level

Always format your responses in a clear, structured way with sections for:
- Project Analysis
- Customer Assessment
- Tool Recommendations
- Safety Considerations
- Success Tips

Be concise but thorough, and always prioritize the customer's safety and success.",
            Kernel = _kernel
        };
    }

    private async Task<PhotoAnalysisResult> AnalyzePhotoAsync(IFormFile image, string prompt)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://analyze-photo-service");
            
            using var content = new MultipartFormDataContent();
            using var imageStream = image.OpenReadStream();
            using var streamContent = new StreamContent(imageStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
            
            content.Add(streamContent, "image", image.FileName);
            content.Add(new StringContent(prompt), "prompt");

            var response = await client.PostAsync("/api/PhotoAnalysis/analyze", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PhotoAnalysisResult>();
                return result ?? CreateFallbackPhotoAnalysis(prompt);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to call AnalyzePhotoService, using fallback");
        }

        return CreateFallbackPhotoAnalysis(prompt);
    }

    private PhotoAnalysisResult CreateFallbackPhotoAnalysis(string prompt)
    {
        return new PhotoAnalysisResult 
        { 
            Description = $"Room analysis for prompt: {prompt}. Detected painted walls with preparation needed.",
            DetectedMaterials = new[] { "paint", "wall", "surface preparation" }
        };
    }

    private async Task<CustomerInformation> GetCustomerInformationAsync(string customerId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://customer-information-service");
            
            var response = await client.GetAsync($"/api/Customer/{customerId}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CustomerInformation>();
                return result ?? CreateFallbackCustomer(customerId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to call CustomerInformationService, using fallback");
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

    private async Task<string> GenerateToolReasoningWithAgentAsync(ChatCompletionAgent agent, PhotoAnalysisResult photoAnalysis, CustomerInformation customer, string prompt)
    {
        try
        {
            var reasoningRequest = new ReasoningRequest
            {
                PhotoAnalysis = photoAnalysis,
                Customer = customer,
                Prompt = prompt
            };

            // First try the dedicated reasoning service
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("http://tool-reasoning-service");
                
                var response = await client.PostAsJsonAsync("/api/Reasoning/generate", reasoningRequest);
                
                if (response.IsSuccessStatusCode)
                {
                    var reasoning = await response.Content.ReadAsStringAsync();
                    return reasoning;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to call ToolReasoningService, using agent fallback");
            }

            // Fallback to using the kernel directly
            var agentPrompt = $@"
You are Zava, an expert DIY and home improvement assistant. Analyze this DIY project and provide tool recommendations:

Project: {prompt}
Image Analysis: {photoAnalysis.Description}
Detected Materials: {string.Join(", ", photoAnalysis.DetectedMaterials)}
Customer Tools: {string.Join(", ", customer.OwnedTools)}
Customer Skills: {string.Join(", ", customer.Skills)}

Provide detailed reasoning for tool recommendations considering their existing tools and skill level. Be encouraging but prioritize safety.";

            try
            {
                var result = await _kernel.InvokePromptAsync(agentPrompt);
                return result.GetValue<string>() ?? GenerateFallbackReasoning(photoAnalysis, customer, prompt);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Kernel invocation failed, using fallback");
                return GenerateFallbackReasoning(photoAnalysis, customer, prompt);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to generate AI reasoning, using fallback");
            return GenerateFallbackReasoning(photoAnalysis, customer, prompt);
        }
    }

    private string GenerateFallbackReasoning(PhotoAnalysisResult photoAnalysis, CustomerInformation customer, string prompt)
    {
        return $"Based on the task '{prompt}' and the detected materials ({string.Join(", ", photoAnalysis.DetectedMaterials)}), specific tools will be recommended to complement your existing tools: {string.Join(", ", customer.OwnedTools)}.";
    }

    private async Task<ToolMatchResult> MatchToolsAsync(string customerId, string[] detectedMaterials, string prompt)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://customer-information-service");
            
            var matchRequest = new ToolMatchRequest
            {
                CustomerId = customerId,
                DetectedMaterials = detectedMaterials,
                Prompt = prompt
            };

            var response = await client.PostAsJsonAsync("/api/Customer/match-tools", matchRequest);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ToolMatchResult>();
                return result ?? CreateFallbackToolMatch();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to call CustomerInformationService for tool matching, using fallback");
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
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://inventory-service");
            
            var skus = tools.Select(t => t.Sku).ToArray();
            var searchRequest = new InventorySearchRequest { Skus = skus };
            
            var response = await client.PostAsJsonAsync("/api/Inventory/search", searchRequest);
            
            if (response.IsSuccessStatusCode)
            {
                var inventoryResults = await response.Content.ReadFromJsonAsync<ToolRecommendation[]>();
                return inventoryResults ?? tools;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to call InventoryService, using fallback");
        }

        return tools; // Return original tools if inventory service fails
    }
}