namespace SharedEntities;

public class NavigationStep
{
    public string Direction { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    // Landmark can be a textual description or a Location object; keep nullable to allow null landmarks
    public object? Landmark { get; set; }
}