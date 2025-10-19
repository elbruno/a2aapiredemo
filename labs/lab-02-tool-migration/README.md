# Lab 02: Tool Migration

**Duration**: 45 minutes  
**Level**: Intermediate  
**Prerequisites**: Lab 01 completed

## Learning Objectives

- Convert complex plugins with database operations
- Handle async functions properly
- Integrate external services with Agent Framework
- Test function calling with multiple tools

## Scenario

Migrate a customer management system that:
- Retrieves customer data from a database
- Creates new customer records
- Updates customer information
- Searches customers by various criteria

## Lab Structure

### Starter Code (`starter/`)

The starter includes:
- `CustomerPlugin.cs` - Semantic Kernel plugin with 5 functions
- `DatabaseService.cs` - Mock database service
- `Program.cs` - Application entry point with Kernel setup

### Your Tasks

1. **Convert async plugin methods to async functions**
   ```csharp
   // Before (Plugin)
   [KernelFunction]
   public async Task<string> GetCustomer(int customerId)
   
   // After (Function)
   async Task<string> GetCustomer(int customerId)
   ```

2. **Handle dependency injection**
   - Database service needs to be accessible to functions
   - Use closures or pass as parameters

3. **Register all tools with agent**
   ```csharp
   Tools = { GetCustomer, CreateCustomer, UpdateCustomer, SearchCustomers, DeleteCustomer }
   ```

4. **Test function calling**
   - Verify AI calls correct functions
   - Confirm async operations complete
   - Check database service integration

## Key Concepts

### Async Functions

Agent Framework supports async functions natively:
```csharp
async Task<string> FetchDataAsync(string url)
{
    using var client = new HttpClient();
    return await client.GetStringAsync(url);
}
```

### Closures for Dependencies

```csharp
var dbService = new DatabaseService();

async Task<string> GetCustomer(int id) =>
    await dbService.GetCustomerAsync(id);
```

### Complex Return Types

```csharp
async Task<CustomerData> GetCustomerDetails(int id)
{
    var customer = await dbService.GetAsync(id);
    return new CustomerData(customer.Name, customer.Email);
}
```

## Verification

Test these scenarios:
1. "Get customer with ID 123"
2. "Create a new customer named John Doe"
3. "Search for customers in Seattle"
4. "Update email for customer 456"

## Solution

Compare with `solution/` when complete. Key changes:
- Plugin class → 5 simple async functions
- Database service accessed via closure
- All functions registered in Tools collection

## Next: [Lab 03: ASP.NET Integration](../lab-03-aspnet-integration/)
