using Microsoft.SemanticKernel;
using System.ComponentModel;

/// <summary>
/// Semantic Kernel plugin providing customer management operations.
/// </summary>
public sealed class CustomerPlugin
{
    private readonly DatabaseService _database;

    public CustomerPlugin(DatabaseService database)
    {
        _database = database;
    }

    [KernelFunction]
    [Description("Get customer information by ID")]
    public async Task<string> GetCustomer(int customerId)
    {
        return await _database.GetCustomerAsync(customerId);
    }

    [KernelFunction]
    [Description("Create a new customer with name, email, and city")]
    public async Task<string> CreateCustomer(string name, string email, string city)
    {
        return await _database.CreateCustomerAsync(name, email, city);
    }

    [KernelFunction]
    [Description("Update customer email or city")]
    public async Task<string> UpdateCustomer(int customerId, string? email = null, string? city = null)
    {
        return await _database.UpdateCustomerAsync(customerId, email, city);
    }

    [KernelFunction]
    [Description("Search for customers by city or name")]
    public async Task<string> SearchCustomers(string? city = null, string? nameContains = null)
    {
        return await _database.SearchCustomersAsync(city, nameContains);
    }

    [KernelFunction]
    [Description("Delete a customer by ID")]
    public async Task<string> DeleteCustomer(int customerId)
    {
        return await _database.DeleteCustomerAsync(customerId);
    }
}
