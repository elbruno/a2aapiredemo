public class MultiAgentResponse
{
    public string OrchestrationId { get; set; } = string.Empty;
    public AgentStep[] Steps { get; set; } = Array.Empty<AgentStep>();
    public ProductAlternative[] Alternatives { get; set; } = Array.Empty<ProductAlternative>();
    public NavigationInstructions? NavigationInstructions { get; set; }
}
