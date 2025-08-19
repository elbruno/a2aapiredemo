namespace SharedEntities;

public class MultiAgentResponse
{
    public string OrchestrationId { get; set; } = string.Empty;
    public AgentStep[] Steps { get; set; } = [];
    public ProductAlternative[] Alternatives { get; set; } = [];
    public NavigationInstructions? NavigationInstructions { get; set; }
}