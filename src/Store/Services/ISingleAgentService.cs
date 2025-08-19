using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace Store.Services;

public interface ISingleAgentService
{
    Task<SingleAgentAnalysisResponse?> AnalyzeAsync(SingleAgentAnalysisRequest request);
}

public class SingleAgentAnalysisRequest
{
    public IBrowserFile Image { get; set; } = null!;
    
    [Required]
    public string Prompt { get; set; } = string.Empty;
    
    [Required]
    public string CustomerId { get; set; } = string.Empty;
}

public class SingleAgentAnalysisResponse
{
    public string Analysis { get; set; } = string.Empty;
    public string[] ReusableTools { get; set; } = Array.Empty<string>();
    public ToolRecommendation[] RecommendedTools { get; set; } = Array.Empty<ToolRecommendation>();
    public string Reasoning { get; set; } = string.Empty;
}

public class ToolRecommendation
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
}