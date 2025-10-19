# Module 10: Options Configuration

**Duration**: 10 minutes  
**Level**: Beginner  

---

## Learning Objectives

- Configure agent options and parameters
- Use `ChatClientAgentOptions` effectively
- Set model parameters (temperature, max tokens)
- Configure for GitHub Models

---

## Agent Options

### Basic Configuration

```csharp
var options = new ChatClientAgentOptions
{
    Name = "MyAgent",
    Instructions = "You are a helpful assistant.",
    Temperature = 0.7f,
    MaxTokens = 1000
};

var agent = new ChatClientAgent(chatClient, options);
```

---

## Available Options

### Core Options

```csharp
new ChatClientAgentOptions
{
    // Identity
    Name = "Assistant",
    
    // System instructions
    Instructions = "You are a helpful AI assistant.",
    
    // Model parameters
    Temperature = 0.7f,      // 0.0 to 1.0 (creativity)
    MaxTokens = 2000,        // Maximum response length
    TopP = 0.9f,            // Nucleus sampling
    
    // Function calling
    Tools = { Function1, Function2 }
}
```

---

## Temperature Settings

Controls randomness/creativity:

```csharp
// Deterministic (factual, consistent)
Temperature = 0.0f

// Balanced (default)
Temperature = 0.7f

// Creative (varied, imaginative)
Temperature = 1.0f
```

**Example:**
```csharp
// For factual Q&A
var factAgent = new ChatClientAgent(chatClient, new()
{
    Temperature = 0.0f,
    Instructions = "Provide accurate, factual answers."
});

// For creative writing
var creativeAgent = new ChatClientAgent(chatClient, new()
{
    Temperature = 0.9f,
    Instructions = "Be creative and imaginative."
});
```

---

## Max Tokens

Limits response length:

```csharp
new ChatClientAgentOptions
{
    MaxTokens = 500  // Short responses
}

new ChatClientAgentOptions
{
    MaxTokens = 4000  // Longer responses
}
```

**Note**: 1 token ≈ 0.75 words

---

## Run-Time Options

Override agent options per request:

```csharp
var agent = new ChatClientAgent(chatClient, new() { Temperature = 0.7f });

// Override for specific request
var response = await agent.RunAsync(
    "Tell me a joke",
    new ChatClientAgentRunOptions
    {
        Temperature = 0.9f,  // More creative for jokes
        MaxTokens = 100
    });
```

---

## Configuration from appsettings.json

```json
{
  "Agent": {
    "Name": "MyAgent",
    "Temperature": 0.7,
    "MaxTokens": 1000
  }
}
```

```csharp
var agentConfig = configuration.GetSection("Agent");
var options = new ChatClientAgentOptions
{
    Name = agentConfig["Name"],
    Temperature = float.Parse(agentConfig["Temperature"]!),
    MaxTokens = int.Parse(agentConfig["MaxTokens"]!)
};
```

---

## GitHub Models Specific

```csharp
// GitHub Models endpoint configuration
var chatClient = new ChatClient(
    "gpt-4o-mini",  // Model selection
    new ApiKeyCredential(githubToken),
    new OpenAIClientOptions 
    { 
        Endpoint = new Uri("https://models.github.ai/inference")
    });

// Agent options work the same
var agent = new ChatClientAgent(chatClient, new()
{
    Temperature = 0.7f,
    MaxTokens = 2000
});
```

---

## Best Practices

1. **Start with defaults** - Temperature 0.7, adjust as needed
2. **Lower temperature for facts** - 0.0-0.3 for accuracy
3. **Higher temperature for creativity** - 0.8-1.0 for stories
4. **Set appropriate max tokens** - Balance response length and cost
5. **Use instructions wisely** - Clear, specific system prompts

---

## Common Configurations

### Customer Support Agent
```csharp
new ChatClientAgentOptions
{
    Name = "SupportAgent",
    Instructions = "You are a helpful customer support agent. Be polite and professional.",
    Temperature = 0.3f,  // Consistent, factual
    MaxTokens = 500
}
```

### Creative Writing Agent
```csharp
new ChatClientAgentOptions
{
    Name = "Writer",
    Instructions = "You are a creative writer. Be imaginative and engaging.",
    Temperature = 0.9f,  // High creativity
    MaxTokens = 2000
}
```

### Code Assistant
```csharp
new ChatClientAgentOptions
{
    Name = "CodeHelper",
    Instructions = "You are a coding assistant. Provide accurate, working code.",
    Temperature = 0.2f,  // Precise, deterministic
    MaxTokens = 1500
}
```

---

## Next: [Module 11: Complete Example](../11-Complete-Example/)
