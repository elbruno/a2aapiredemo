using System.ComponentModel.DataAnnotations;

namespace StoreRealtime.Services;

public interface ISystemPromptService
{
    string GetCurrentPrompt();
    void SetPrompt(string prompt);
    string GetDefaultPrompt();
    event Action? OnPromptChanged;
}

public class SystemPromptService : ISystemPromptService
{
    private const string DefaultSystemPrompt = """
        You are a useful assistant.
        Respond as succinctly as possible, in just a few words.
        Check the product database and external sources for information.
        The current date is {0}
        """;

    private string _currentPrompt;
    
    public event Action? OnPromptChanged;

    public SystemPromptService()
    {
        _currentPrompt = GetDefaultPrompt();
    }

    public string GetCurrentPrompt()
    {
        return string.Format(_currentPrompt, DateTime.Now.ToLongDateString());
    }

    public void SetPrompt(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            _currentPrompt = GetDefaultPrompt();
        }
        else
        {
            _currentPrompt = prompt;
        }
        
        OnPromptChanged?.Invoke();
    }

    public string GetDefaultPrompt()
    {
        return string.Format(DefaultSystemPrompt, DateTime.Now.ToLongDateString());
    }
}

public class SystemPromptModel
{
    [Required]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "System prompt must be between 10 and 2000 characters.")]
    public string Prompt { get; set; } = "";
    
    public string DisplayPrompt => Prompt.Contains("{0}") ? string.Format(Prompt, DateTime.Now.ToLongDateString()) : Prompt;
}