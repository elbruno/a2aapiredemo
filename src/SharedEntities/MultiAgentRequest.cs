namespace SharedEntities;

public class MultiAgentRequest
{
    public string UserId { get; set; } = string.Empty;
    public string ProductQuery { get; set; } = string.Empty;
    public Location? Location { get; set; }
}