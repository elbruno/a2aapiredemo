# Module 06: Tool Registration (Plugins → Functions)

**Duration**: 20 minutes  
**Level**: Intermediate  
**Prerequisites**: Module 04 and 05 completed

---

## Learning Objectives

- Migrate Semantic Kernel plugins to Agent Framework functions
- Understand direct function registration
- Remove `[KernelFunction]` attributes
- Integrate functions with GitHub Models agents

---

## Overview

One of the biggest changes in Agent Framework is tool registration. Semantic Kernel uses plugin classes with `[KernelFunction]` attributes, while Agent Framework uses direct function registration.

---

## Semantic Kernel Approach (Before)

### Plugin Class with Attributes

```csharp
using Microsoft.SemanticKernel;
using System.ComponentModel;

public class WeatherPlugin
{
    [KernelFunction]
    [Description("Get current weather for a location")]
    public string GetWeather(
        [Description("City name")] string location)
    {
        // Implementation
        return $"Weather in {location}: Sunny, 72°F";
    }

    [KernelFunction]
    [Description("Get weather forecast")]
    public string GetForecast(
        [Description("City name")] string location,
        [Description("Number of days")] int days)
    {
        return $"{days}-day forecast for {location}: Mostly sunny";
    }
}

// Registration
kernel.Plugins.Add(KernelPluginFactory.CreateFromType<WeatherPlugin>());
```

---

## Agent Framework Approach (After)

### Direct Function Registration

```csharp
// AF: Simple function definitions (no attributes needed)
string GetWeather(string location)
{
    return $"Weather in {location}: Sunny, 72°F";
}

string GetForecast(string location, int days)
{
    return $"{days}-day forecast for {location}: Mostly sunny";
}

// Registration: Pass functions directly to agent
var agent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "WeatherAssistant",
        Instructions = "You are a weather assistant.",
        Tools = { GetWeather, GetForecast }
    });
```

**Key Change**: No plugin class, no attributes, just direct function references!

---

## Complete Example with GitHub Models

```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.ClientModel;

class Program
{
    static async Task Main(string[] args)
    {
        // Configuration
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var githubToken = config["GITHUB_TOKEN"]!;

        // Create chat client
        var chatClient = new ChatClient(
            "gpt-4o-mini",
            new ApiKeyCredential(githubToken),
            new OpenAIClientOptions { 
                Endpoint = new Uri("https://models.github.ai/inference") 
            });

        // Define functions (no attributes needed!)
        string GetWeather(string location) => 
            $"Current weather in {location}: Sunny, 72°F";

        int AddNumbers(int a, int b) => a + b;

        DateTime GetCurrentTime() => DateTime.Now;

        // Create agent with tools
        var agent = new ChatClientAgent(
            chatClient.AsIChatClient(),
            new ChatClientAgentOptions
            {
                Name = "ToolAgent",
                Instructions = "You can check weather, add numbers, and tell the time.",
                Tools = { GetWeather, AddNumbers, GetCurrentTime }
            });

        // Test the agent
        var response1 = await agent.RunAsync("What's the weather in Seattle?");
        Console.WriteLine($"Response: {response1.Text}\n");

        var response2 = await agent.RunAsync("What's 42 + 58?");
        Console.WriteLine($"Response: {response2.Text}\n");

        var response3 = await agent.RunAsync("What time is it?");
        Console.WriteLine($"Response: {response3.Text}");
    }
}
```

---

## Migration Steps

### Step 1: Extract Function Logic

**Before (SK Plugin):**
```csharp
public class MathPlugin
{
    [KernelFunction]
    public int Add(int a, int b) => a + b;
}
```

**After (AF Function):**
```csharp
int Add(int a, int b) => a + b;
```

### Step 2: Remove Attributes

- Remove `[KernelFunction]`
- Remove `[Description]` attributes
- Remove `using System.ComponentModel;`

### Step 3: Register Directly

```csharp
var agent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Tools = { Add, Subtract, Multiply }
    });
```

---

## Advanced Function Patterns

### With Description (if needed)

```csharp
[Description("Calculates the sum of two numbers")]
int Add([Description("First number")] int a, [Description("Second number")] int b) 
    => a + b;
```

**Note**: Descriptions are optional in AF but can help the AI understand function purpose.

### Async Functions

```csharp
async Task<string> FetchDataAsync(string url)
{
    using var client = new HttpClient();
    return await client.GetStringAsync(url);
}

// Register async function
var agent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Tools = { FetchDataAsync }
    });
```

### Functions with Complex Types

```csharp
record WeatherData(string Location, int Temperature, string Conditions);

WeatherData GetDetailedWeather(string location)
{
    return new WeatherData(location, 72, "Sunny");
}
```

---

## Common Patterns

### Multiple Related Functions

```csharp
// Calculator functions
int Add(int a, int b) => a + b;
int Subtract(int a, int b) => a - b;
int Multiply(int a, int b) => a * b;
double Divide(int a, int b) => (double)a / b;

var agent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "Calculator",
        Instructions = "You are a calculator assistant.",
        Tools = { Add, Subtract, Multiply, Divide }
    });
```

### Database Operations

```csharp
string GetUser(int userId) => 
    $"User {userId}: John Doe (john@example.com)";

string[] ListUsers() => 
    new[] { "User 1: John", "User 2: Jane", "User 3: Bob" };

bool CreateUser(string name, string email) => true;
```

---

## Comparison Table

| Aspect | Semantic Kernel | Agent Framework |
|--------|----------------|-----------------|
| **Definition** | Plugin class | Direct function |
| **Attributes** | `[KernelFunction]` required | Optional |
| **Registration** | `kernel.Plugins.Add()` | Add to `Tools` collection |
| **Complexity** | High (class + attributes) | Low (just functions) |
| **Flexibility** | Must be in plugin class | Any function works |

---

## Benefits of Direct Registration

1. **Simpler Code** - No plugin classes needed
2. **Less Boilerplate** - No attributes required
3. **More Flexible** - Use any function
4. **Easier Testing** - Functions are plain C#
5. **Better IntelliSense** - Standard function signatures

---

## Best Practices

1. **Keep Functions Simple** - Each function should do one thing
2. **Use Clear Names** - Function names help AI understand purpose
3. **Return Useful Data** - Provide data the AI can work with
4. **Handle Errors** - Include try-catch for external operations
5. **Add Descriptions** - Optional but helpful for complex functions

---

## Complete Migration Example

### Before (Semantic Kernel)

```csharp
public class ToolsPlugin
{
    [KernelFunction]
    [Description("Get current temperature")]
    public string GetTemperature(string city) => $"{city}: 72°F";

    [KernelFunction]
    public int Calculate(int x, int y) => x + y;
}

kernel.Plugins.Add(KernelPluginFactory.CreateFromType<ToolsPlugin>());
var agent = new ChatCompletionAgent { Kernel = kernel };
```

### After (Agent Framework)

```csharp
string GetTemperature(string city) => $"{city}: 72°F";
int Calculate(int x, int y) => x + y;

var agent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Tools = { GetTemperature, Calculate }
    });
```

---

## Next Steps

- [Module 07: Invocation Patterns](../07-Invocation-Patterns/)
- [Module 08: Streaming Responses](../08-Streaming-Responses/)
- [Module 11: Complete Example](../11-Complete-Example/) - Full chatbot with tools

---

**Ready for invocation patterns?** Continue to [Module 07: Invocation Patterns](../07-Invocation-Patterns/)! ⚡
