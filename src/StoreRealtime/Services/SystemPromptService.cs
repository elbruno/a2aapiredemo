namespace StoreRealtime.Services;

public class SystemPromptService
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
