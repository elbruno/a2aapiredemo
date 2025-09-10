using System.ComponentModel.DataAnnotations;

namespace StoreRealtime.Services;

public class SystemPromptModel
{
    [Required]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "System prompt must be between 10 and 2000 characters.")]
    public string Prompt { get; set; } = "";
    
    public string DisplayPrompt => Prompt.Contains("{0}") ? string.Format(Prompt, DateTime.Now.ToLongDateString()) : Prompt;
}