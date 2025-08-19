using System.ComponentModel.DataAnnotations;

public class MultiAgentRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string ProductQuery { get; set; } = string.Empty;

    public Location? Location { get; set; }
}
