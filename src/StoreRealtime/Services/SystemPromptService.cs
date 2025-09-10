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
    private int _relevanceThreshold;
    
    public event Action? OnPromptChanged;
    public event Action? OnRelevanceThresholdChanged;

    public SystemPromptService()
    {
        _currentPrompt = GetDefaultPrompt();
        _relevanceThreshold = 80; // Default 80%
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

    public int GetRelevanceThreshold()
    {
        return _relevanceThreshold;
    }

    public void SetRelevanceThreshold(int threshold)
    {
        if (threshold < 0) threshold = 0;
        if (threshold > 100) threshold = 100;
        
        _relevanceThreshold = threshold;
        OnRelevanceThresholdChanged?.Invoke();
    }
}
