using System.ComponentModel.DataAnnotations;

namespace StoreRealtime.Services;

public class SystemPromptModel
{
    [Required]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "System prompt must be between 10 and 2000 characters.")]
    public string Prompt { get; set; } = "";
    
    [Range(0, 100, ErrorMessage = "Relevance threshold must be between 0 and 100.")]
    public int RelevanceThreshold { get; set; } = 80;
    
    public string DisplayPrompt => Prompt.Contains("{0}") ? string.Format(Prompt, DateTime.Now.ToLongDateString()) : Prompt;
}