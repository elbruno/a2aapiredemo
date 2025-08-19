namespace MultiAgentDemo.Models;

public class MultiAgentRequest
{
    public string UserId { get; set; } = string.Empty;
    public string ProductQuery { get; set; } = string.Empty;
    public Location? Location { get; set; }
}

public class Location
{
    public double Lat { get; set; }
    public double Lon { get; set; }
}

public class MultiAgentResponse
{
    public string OrchestrationId { get; set; } = string.Empty;
    public AgentStep[] Steps { get; set; } = Array.Empty<AgentStep>();
    public ProductAlternative[] Alternatives { get; set; } = Array.Empty<ProductAlternative>();
    public NavigationInstructions? NavigationInstructions { get; set; }
}

public class AgentStep
{
    public string Agent { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ProductAlternative
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool InStock { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Aisle { get; set; }
    public string Section { get; set; } = string.Empty;
}

public class NavigationInstructions
{
    public string StartLocation { get; set; } = string.Empty;
    public NavigationStep[] Steps { get; set; } = Array.Empty<NavigationStep>();
    public string EstimatedTime { get; set; } = string.Empty;
}

public class NavigationStep
{
    public string Direction { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Landmark { get; set; } = string.Empty;
}