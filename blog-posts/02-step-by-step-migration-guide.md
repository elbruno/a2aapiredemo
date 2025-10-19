---
title: "Step-by-Step Guide: Migrating from Semantic Kernel to Agent Framework"
date: 2024-01-22
author: "Bruno Capuano"
description: "Complete step-by-step guide for migrating your Semantic Kernel application to Microsoft Agent Framework with GitHub Models and .NET 9"
keywords: ["Migration Guide", "Semantic Kernel", "Agent Framework", ".NET 9", "GitHub Models", "Tutorial", "C#"]
tags: ["AI", "Migration", ".NET", "C#", "Tutorial", "GitHub Models"]
---

# Step-by-Step Guide: Migrating from Semantic Kernel to Agent Framework

## Introduction

In this comprehensive guide, we'll walk through migrating a real Semantic Kernel application to Microsoft Agent Framework. We'll use GitHub Models for free AI access during development, then show how to switch to production endpoints.

## What We're Building

We'll migrate a customer support chatbot that:
- Answers customer questions
- Checks order status
- Processes refund requests
- Maintains conversation context

**Starting Point**: Semantic Kernel with OpenAI
**End Goal**: Agent Framework with GitHub Models

## Step 1: Understand Your Current Application

### Current Semantic Kernel Code

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

// Plugin class
public class SupportPlugin
{
    [KernelFunction]
    [Description("Get order status")]
    public string GetOrderStatus(string orderId)
    {
        return $"Order {orderId}: Shipped, arriving Tuesday";
    }

    [KernelFunction]
    [Description("Process refund")]
    public string ProcessRefund(string orderId)
    {
        return $"Refund approved for order {orderId}";
    }
}

// Application setup
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4", apiKey)
    .Build();

kernel.Plugins.Add(KernelPluginFactory.CreateFromType<SupportPlugin>());

var agent = new ChatCompletionAgent
{
    Kernel = kernel,
    Name = "SupportBot",
    Instructions = "You are a helpful customer support agent."
};

// Usage
await foreach (var item in agent.InvokeAsync(thread, userMessage))
{
    Console.WriteLine(item.Content);
}
```

## Step 2: Update NuGet Packages

### Remove Old Packages

```bash
dotnet remove package Microsoft.SemanticKernel
dotnet remove package Microsoft.SemanticKernel.Agents.Core
```

### Add New Packages

```bash
dotnet add package Microsoft.Extensions.AI --version 9.10.0
dotnet add package Microsoft.Extensions.AI.OpenAI --prerelease
dotnet add package Microsoft.Agents.AI --prerelease
dotnet add package Microsoft.Extensions.Configuration.UserSecrets
```

### Updated .csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <UserSecretsId>support-bot-12345</UserSecretsId>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.AI" Version="9.10.0" />
    <PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="9.10.0-preview.1.25513.3" />
    <PackageReference Include="Microsoft.Agents.AI" Version="1.0.0-preview.251001.1" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="9.0.0" />
  </ItemGroup>
</Project>
```

## Step 3: Set Up GitHub Models

### Get Your GitHub Token

1. Go to https://github.com/settings/tokens
2. Click "Generate new token (classic)"
3. Select scopes: `repo`, `read:org`
4. Generate and copy the token

### Configure User Secrets

```bash
# Initialize user secrets (if not already done)
dotnet user-secrets init

# Store your GitHub token
dotnet user-secrets set "GITHUB_TOKEN" "ghp_your_token_here"
```

## Step 4: Update Namespace Imports

### Before (Semantic Kernel)

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;
```

### After (Agent Framework)

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.ClientModel;
```

## Step 5: Convert Plugins to Functions

### Before (Plugin Class)

```csharp
public class SupportPlugin
{
    [KernelFunction]
    [Description("Get order status")]
    public string GetOrderStatus(string orderId)
    {
        return $"Order {orderId}: Shipped, arriving Tuesday";
    }

    [KernelFunction]
    [Description("Process refund")]
    public string ProcessRefund(string orderId)
    {
        return $"Refund approved for order {orderId}";
    }
}
```

### After (Direct Functions)

```csharp
// Simple function definitions - no class, no attributes!
string GetOrderStatus(string orderId)
{
    return $"Order {orderId}: Shipped, arriving Tuesday";
}

string ProcessRefund(string orderId)
{
    return $"Refund approved for order {orderId}";
}
```

## Step 6: Create Chat Client with GitHub Models

```csharp
// Load configuration
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var githubToken = configuration["GITHUB_TOKEN"] 
    ?? throw new InvalidOperationException("GITHUB_TOKEN not configured");

// Create chat client for GitHub Models
var chatClient = new ChatClient(
    "gpt-4o-mini",  // Free model from GitHub Models
    new ApiKeyCredential(githubToken),
    new OpenAIClientOptions 
    { 
        Endpoint = new Uri("https://models.github.ai/inference") 
    });
```

## Step 7: Create Agent with Tools

### Replace Kernel + Agent with ChatClientAgent

```csharp
var agent = new ChatClientAgent(
    chatClient.AsIChatClient(),
    new ChatClientAgentOptions
    {
        Name = "SupportBot",
        Instructions = "You are a helpful customer support agent. Be professional and friendly.",
        Temperature = 0.7f,
        MaxTokens = 1000,
        Tools = { GetOrderStatus, ProcessRefund }  // Direct function references!
    });
```

## Step 8: Update Invocation Pattern

### Before (Semantic Kernel)

```csharp
await foreach (var item in agent.InvokeAsync(thread, userMessage))
{
    var content = item.Content.GetValue<ChatMessageContent>();
    Console.WriteLine(content.Content);
}
```

### After (Agent Framework)

```csharp
var response = await agent.RunAsync(userMessage);
Console.WriteLine(response.Text);
```

Much simpler! Direct access to response text.

## Step 9: Complete Migrated Application

Here's the full migrated code:

```csharp
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.ClientModel;

class SupportBot
{
    static async Task Main(string[] args)
    {
        // Configuration
        var config = new ConfigurationBuilder()
            .AddUserSecrets<SupportBot>()
            .Build();

        var githubToken = config["GITHUB_TOKEN"]!;

        // Define support functions
        string GetOrderStatus(string orderId) =>
            $"Order {orderId}: Shipped, arriving Tuesday";

        string ProcessRefund(string orderId) =>
            $"Refund approved for order {orderId}";

        // Create chat client with GitHub Models
        var chatClient = new ChatClient(
            "gpt-4o-mini",
            new ApiKeyCredential(githubToken),
            new OpenAIClientOptions { 
                Endpoint = new Uri("https://models.github.ai/inference") 
            });

        // Create agent
        var agent = new ChatClientAgent(
            chatClient.AsIChatClient(),
            new ChatClientAgentOptions
            {
                Name = "SupportBot",
                Instructions = "You are a customer support agent. Be helpful and professional.",
                Temperature = 0.7f,
                Tools = { GetOrderStatus, ProcessRefund }
            });

        // Interactive chat loop
        Console.WriteLine("Customer Support Bot (type 'exit' to quit)\n");

        while (true)
        {
            Console.Write("Customer: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit")
                break;

            try
            {
                var response = await agent.RunAsync(input);
                Console.WriteLine($"Bot: {response.Text}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }
    }
}
```

## Step 10: Test Your Application

```bash
dotnet run
```

Example conversation:
```
Customer Support Bot (type 'exit' to quit)

Customer: What's the status of order 12345?
Bot: Let me check that for you. Order 12345 has been shipped and is arriving Tuesday!

Customer: I need a refund for that order
Bot: I've processed your refund request. The refund for order 12345 has been approved and will be processed within 3-5 business days.

Customer: Thank you!
Bot: You're welcome! Is there anything else I can help you with today?

Customer: exit
```

## Step 11: Switch to Production (When Ready)

When you're ready to deploy, simply change the endpoint:

### Development (GitHub Models - Free)
```csharp
var chatClient = new ChatClient(
    "gpt-4o-mini",
    new ApiKeyCredential(githubToken),
    new OpenAIClientOptions { 
        Endpoint = new Uri("https://models.github.ai/inference") 
    });
```

### Production (OpenAI)
```csharp
var chatClient = new ChatClient(
    "gpt-4o-mini",
    new ApiKeyCredential(openAIKey));
```

Or Azure OpenAI:
```csharp
var chatClient = new ChatClient(
    deployment: "gpt-4o-mini",
    endpoint: new Uri("https://your-resource.openai.azure.com/"),
    credential: new AzureKeyCredential(azureKey));
```

## Code Reduction Summary

Let's compare the line counts:

| Component | Semantic Kernel | Agent Framework | Reduction |
|-----------|----------------|-----------------|-----------|
| Package setup | 15 lines | 10 lines | 33% |
| Imports | 8 lines | 6 lines | 25% |
| Function definitions | 18 lines | 6 lines | 67% |
| Client/Kernel setup | 8 lines | 7 lines | 12% |
| Agent creation | 10 lines | 11 lines | -10% |
| Usage | 6 lines | 3 lines | 50% |
| **Total** | **65 lines** | **43 lines** | **34%** |

## Common Migration Issues

### Issue 1: Missing GitHub Token

**Error**: `InvalidOperationException: GITHUB_TOKEN not configured`

**Solution**: Run `dotnet user-secrets set "GITHUB_TOKEN" "your-token"`

### Issue 2: Package Version Conflicts

**Error**: "Package downgrade detected"

**Solution**: Use exact versions specified in Step 2

### Issue 3: Namespace Not Found

**Error**: `OpenAI.Chat` namespace not found

**Solution**: Add `Microsoft.Extensions.AI.OpenAI` package

## Practice with Hands-On Labs

Now that you understand the migration process, try these hands-on labs:

1. **[Lab 01: Basic Migration](../labs/lab-01-basic-migration/)** - Practice this exact migration yourself
   - [Starter Code (SK)](../labs/lab-01-basic-migration/starter/before-sk/)
   - [Solution Code (AF)](../labs/lab-01-basic-migration/solution/after-af/)

2. **[Lab 02: Tool Migration](../labs/lab-02-tool-migration/)** - Handle complex async functions and database operations
   - Learn closure patterns for dependencies
   - Work with multiple async tools
   - [View Code Samples](../labs/lab-02-tool-migration/)

3. **[Lab 03: ASP.NET Integration](../labs/lab-03-aspnet-integration/)** - Build production-ready web APIs
   - Environment-based configuration
   - Health checks and monitoring
   - [View Code Samples](../labs/lab-03-aspnet-integration/)

4. **[Lab 04: Testing Strategies](../labs/lab-04-testing-strategies/)** - Write comprehensive tests
   - Unit tests for functions
   - Integration tests with GitHub Models
   - [View Code Samples](../labs/lab-04-testing-strategies/)

## Next Steps

Continue your learning journey:

1. **Add more functionality** - See [Module 06: Tool Registration](../modules/06-Tool-Registration/)
2. **Add streaming** - See [Module 08: Streaming Responses](../modules/08-Streaming-Responses/)
3. **Deploy to production** - See [Module 14: Production Deployment](../modules/14-Production-Deployment/)
4. **Integrate with ASP.NET Core** - See [Module 15: ASP.NET Core](../modules/15-ASPNetCore-Integration/)

## Conclusion

Congratulations! You've successfully migrated from Semantic Kernel to Agent Framework. Your code is now simpler, more maintainable, and uses free GitHub Models for development.

The migration process follows a predictable pattern:
1. Update packages
2. Update namespaces
3. Convert plugins to functions
4. Replace Kernel with ChatClient
5. Update agent creation
6. Simplify invocation

With these steps mastered, you can migrate any Semantic Kernel application to Agent Framework.

## Resources

- [Complete Migration Repository](../README.md)
- [All 15 Learning Modules](../modules/)
- [Hands-On Labs with Code](../labs/)
- [Quick Reference Guide](../docs/QUICK-REFERENCE.md)
- [FAQ](../docs/FAQ.md)

---

**Next Post**: [Real-World Migration Examples](./03-real-world-migration-examples.md) - See how teams migrated production applications.

---

*Questions? Issues? Open a discussion in the repository!*
