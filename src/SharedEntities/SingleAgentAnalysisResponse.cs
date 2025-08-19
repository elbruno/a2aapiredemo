namespace SharedEntities;

public class SingleAgentAnalysisResponse
{
    public string Analysis { get; set; } = string.Empty;
    public string[] ReusableTools { get; set; } = [];
    public ToolRecommendation[] RecommendedTools { get; set; } = [];
    public string Reasoning { get; set; } = string.Empty;
}