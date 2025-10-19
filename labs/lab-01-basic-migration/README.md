# Lab 01: Basic Migration

**Duration**: 30 minutes  
**Level**: Beginner  
**Prerequisites**: .NET 9 SDK, GitHub token

## Learning Objectives

By completing this lab, you will:
- Migrate a Semantic Kernel chatbot to Agent Framework
- Update NuGet packages correctly
- Convert a simple plugin to direct functions
- Configure GitHub Models for free AI access
- Test your migrated application

## Scenario

You have a weather chatbot built with Semantic Kernel that:
- Provides weather information for cities
- Converts temperatures between Celsius and Fahrenheit
- Maintains conversation context

Your task: Migrate it to Agent Framework using GitHub Models.

## Step 1: Review the Starting Code

Navigate to the `starter/` directory and examine the Semantic Kernel implementation.

**Current implementation uses**:
- `Microsoft.SemanticKernel` package
- `Kernel` orchestrator
- Plugin class with `[KernelFunction]` attributes
- OpenAI API (costs money!)

## Step 2: Update Packages

### Task 2.1: Remove Old Packages

Edit `starter/WeatherBot.csproj` and remove:
```xml
<PackageReference Include="Microsoft.SemanticKernel" Version="1.61.0" />
```

### Task 2.2: Add New Packages

Add these package references:
```xml
<PackageReference Include="Microsoft.Extensions.AI" Version="9.10.0" />
<PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="9.10.0-preview.1.25513.3" />
<PackageReference Include="Microsoft.Agents.AI" Version="1.0.0-preview.251001.1" />
<PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="9.0.0" />
```

### Task 2.3: Restore Packages

```bash
cd starter
dotnet restore
```

**Checkpoint**: You should see no restore errors.

## Step 3: Configure GitHub Token

### Task 3.1: Initialize User Secrets

```bash
dotnet user-secrets init
```

### Task 3.2: Set GitHub Token

Get your token from: https://github.com/settings/tokens

```bash
dotnet user-secrets set "GITHUB_TOKEN" "your-github-token-here"
```

## Step 4: Update Using Statements

### Task 4.1: Replace Semantic Kernel Imports

Find these lines in `Program.cs`:
```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using System.ComponentModel;
```

Replace with:
```csharp
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.ClientModel;
```

## Step 5: Convert Plugin to Functions

### Task 5.1: Extract Function Logic

Find the `WeatherPlugin` class:
```csharp
public class WeatherPlugin
{
    [KernelFunction]
    [Description("Get weather for a city")]
    public string GetWeather(string city) 
    {
        return $"Weather in {city}: Sunny, 72°F";
    }

    [KernelFunction]
    [Description("Convert Celsius to Fahrenheit")]
    public string ConvertToFahrenheit(double celsius)
    {
        var fahrenheit = (celsius * 9/5) + 32;
        return $"{celsius}°C = {fahrenheit}°F";
    }
}
```

### Task 5.2: Convert to Simple Functions

Replace the entire plugin class with:
```csharp
// Simple function definitions (no class, no attributes!)
string GetWeather(string city) =>
    $"Weather in {city}: Sunny, 72°F";

string ConvertToFahrenheit(double celsius)
{
    var fahrenheit = (celsius * 9/5) + 32;
    return $"{celsius}°C = {fahrenheit}°F";
}
```

**Hint**: Place these functions at the top of your `Main` method or as local functions.

## Step 6: Replace Kernel with ChatClient

### Task 6.1: Remove Kernel Creation

Find and delete:
```csharp
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4", apiKey)
    .Build();

kernel.Plugins.Add(KernelPluginFactory.CreateFromType<WeatherPlugin>());
```

### Task 6.2: Create ChatClient with GitHub Models

Add this code instead:
```csharp
// Load configuration
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var githubToken = configuration["GITHUB_TOKEN"] 
    ?? throw new InvalidOperationException("GITHUB_TOKEN not set");

// Create chat client with GitHub Models (free!)
var chatClient = new ChatClient(
    "gpt-4o-mini",
    new ApiKeyCredential(githubToken),
    new OpenAIClientOptions { 
        Endpoint = new Uri("https://models.github.ai/inference") 
    });
```

**Hint**: Make sure your function definitions are accessible in the scope where you create the agent.

## Step 7: Create Agent

### Task 7.1: Replace ChatCompletionAgent

Find and delete:
```csharp
var agent = new ChatCompletionAgent
{
    Kernel = kernel,
    Name = "WeatherBot",
    Instructions = "You are a helpful weather assistant."
};
```

### Task 7.2: Create ChatClientAgent

Add:
```csharp
var agent = new ChatClientAgent(
    chatClient.AsIChatClient(),
    new ChatClientAgentOptions
    {
        Name = "WeatherBot",
        Instructions = "You are a helpful weather assistant. Use the provided functions to answer questions.",
        Tools = { GetWeather, ConvertToFahrenheit }
    });
```

## Step 8: Update Invocation Pattern

### Task 8.1: Replace InvokeAsync with RunAsync

Find:
```csharp
await foreach (var item in agent.InvokeAsync(thread, input))
{
    Console.WriteLine(item.Content);
}
```

Replace with:
```csharp
var response = await agent.RunAsync(input);
Console.WriteLine(response.Text);
```

Much simpler!

## Step 9: Test Your Migration

### Task 9.1: Build

```bash
dotnet build
```

**Expected**: Build succeeds with no errors.

### Task 9.2: Run

```bash
dotnet run
```

### Task 9.3: Test Scenarios

Try these inputs:

1. **Weather Query**: "What's the weather in Seattle?"
   - **Expected**: Bot uses GetWeather function and responds with weather info

2. **Temperature Conversion**: "Convert 25 Celsius to Fahrenheit"
   - **Expected**: Bot uses ConvertToFahrenheit function and shows conversion

3. **Context**: 
   - First: "What's the weather in Tokyo?"
   - Then: "What about Paris?"
   - **Expected**: Bot remembers context and provides Paris weather

4. **Exit**: Type "exit" to quit

## Step 10: Compare with Solution

If you got stuck, compare your code with the `solution/` directory.

### Key Differences

| Aspect | Before (SK) | After (AF) |
|--------|-------------|------------|
| Packages | Microsoft.SemanticKernel | Microsoft.Extensions.AI, Microsoft.Agents.AI |
| API Cost | OpenAI (paid) | GitHub Models (free) |
| Plugin | Class with attributes | Simple functions |
| Setup | Kernel + Agent | ChatClient + Agent |
| Invocation | InvokeAsync loop | RunAsync direct |
| Lines of Code | ~80 lines | ~50 lines |

## Bonus Challenges

If you finish early, try these:

### Challenge 1: Add New Function

Add a function that gets the forecast:
```csharp
string GetForecast(string city, int days) =>
    $"{days}-day forecast for {city}: Mostly sunny";
```

Don't forget to add it to the `Tools` collection!

### Challenge 2: Add Error Handling

Wrap the RunAsync call in try-catch:
```csharp
try
{
    var response = await agent.RunAsync(input);
    Console.WriteLine($"Bot: {response.Text}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

### Challenge 3: Add Streaming

Replace `RunAsync` with `RunStreamingAsync`:
```csharp
await foreach (var update in agent.RunStreamingAsync(input))
{
    Console.Write(update.Text);
}
Console.WriteLine();
```

## Verification Checklist

Before moving to the next lab:

- [ ] Code compiles without errors
- [ ] Application runs and responds to queries
- [ ] GitHub Models endpoint is being used (free!)
- [ ] Functions are called correctly
- [ ] Conversation context is maintained
- [ ] No Kernel orchestrator in code
- [ ] No plugin classes with attributes

## What You Learned

✅ Updated NuGet packages from SK to AF  
✅ Converted plugin class to simple functions  
✅ Set up GitHub Models for free AI access  
✅ Created ChatClient and ChatClientAgent  
✅ Simplified invocation pattern  
✅ Reduced code by ~40%

## Next Steps

- **[Lab 02: Tool Migration](../lab-02-tool-migration/)** - Work with complex tools and async functions
- **[Module 06: Tool Registration](../../modules/06-Tool-Registration/)** - Deep dive into function patterns
- **[Quick Reference](../../docs/QUICK-REFERENCE.md)** - API cheat sheet

## Need Help?

- Check the `solution/` directory
- Review [Module 02: Basic Migration](../../modules/02-Basic-Migration/)
- Ask in GitHub Discussions

---

**Congratulations!** You've completed your first migration from Semantic Kernel to Agent Framework! 🎉

**Next Lab**: [Lab 02: Tool Migration](../lab-02-tool-migration/)
