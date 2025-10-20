# Lab 02 Starter: Customer Management Bot (Semantic Kernel)

## Overview

This is the starting point for Lab 02. It demonstrates a Semantic Kernel implementation with:
- Complex plugin with async database operations
- Multiple tool functions (CRUD operations)
- Database service dependency injection
- GitHub Models integration

## Prerequisites

- .NET 9 SDK
- GitHub personal access token

## Configuration

### 1. Initialize User Secrets

```bash
cd labs/lab-02-tool-migration/starter/before-sk
dotnet user-secrets init
```

### 2. Set GitHub Token

```bash
dotnet user-secrets set "GITHUB_TOKEN" "your-github-token-here"
```

Get your token from: https://github.com/settings/tokens

## How to Run

```bash
dotnet build
dotnet run
```

## Sample Commands

Try these commands to test the bot:
- "Get customer 123"
- "Create a new customer named David Lee with email david@example.com in Boston"
- "Search for customers in Seattle"
- "Update email for customer 456 to newemail@example.com"
- "Delete customer 789"

## Key Concepts

### Plugin Class with Dependencies
```csharp
public sealed class CustomerPlugin
{
    private readonly DatabaseService _database;
    
    public CustomerPlugin(DatabaseService database)
    {
        _database = database;
    }
    
    [KernelFunction]
    public async Task<string> GetCustomer(int customerId)
    {
        return await _database.GetCustomerAsync(customerId);
    }
}
```

### Kernel Plugin Registration
```csharp
var database = new DatabaseService();
var plugin = new CustomerPlugin(database);
kernel.ImportPluginFromObject(plugin, "CustomerManagement");
```

## Your Task

Migrate this application to Agent Framework in the `solution/after-af/` directory. Focus on:
1. Converting the plugin class to simple async functions
2. Using closures to access the database service
3. Registering functions with the agent
4. Simplifying the invocation pattern

Compare your solution with `solution/after-af/` when complete.
