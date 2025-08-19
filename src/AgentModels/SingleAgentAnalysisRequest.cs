using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

public class SingleAgentAnalysisRequest
{
    public IBrowserFile Image { get; set; } = null!;

    [Required]
    public string Prompt { get; set; } = string.Empty;

    [Required]
    public string CustomerId { get; set; } = string.Empty;
}
