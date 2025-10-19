/// <summary>
/// Mock database service for customer data operations.
/// In a real application, this would connect to a database.
/// </summary>
public class DatabaseService
{
    private readonly Dictionary<int, Customer> _customers = new()
    {
        { 123, new Customer(123, "Alice Johnson", "alice@example.com", "Seattle") },
        { 456, new Customer(456, "Bob Smith", "bob@example.com", "Portland") },
        { 789, new Customer(789, "Carol Williams", "carol@example.com", "Seattle") }
    };

    public async Task<string> GetCustomerAsync(int customerId)
    {
        await Task.Delay(50); // Simulate database latency
        
        if (_customers.TryGetValue(customerId, out var customer))
        {
            return $"Customer {customerId}: {customer.Name} ({customer.Email}) from {customer.City}";
        }
        return $"Customer {customerId} not found";
    }

    public async Task<string> CreateCustomerAsync(string name, string email, string city)
    {
        await Task.Delay(50);
        
        var id = _customers.Keys.Max() + 1;
        _customers[id] = new Customer(id, name, email, city);
        return $"Created customer {id}: {name}";
    }

    public async Task<string> UpdateCustomerAsync(int customerId, string? email = null, string? city = null)
    {
        await Task.Delay(50);
        
        if (_customers.TryGetValue(customerId, out var customer))
        {
            var updated = customer with 
            { 
                Email = email ?? customer.Email,
                City = city ?? customer.City
            };
            _customers[customerId] = updated;
            return $"Updated customer {customerId}";
        }
        return $"Customer {customerId} not found";
    }

    public async Task<string> SearchCustomersAsync(string? city = null, string? nameContains = null)
    {
        await Task.Delay(50);
        
        var results = _customers.Values.AsEnumerable();
        
        if (!string.IsNullOrEmpty(city))
        {
            results = results.Where(c => c.City.Equals(city, StringComparison.OrdinalIgnoreCase));
        }
        
        if (!string.IsNullOrEmpty(nameContains))
        {
            results = results.Where(c => c.Name.Contains(nameContains, StringComparison.OrdinalIgnoreCase));
        }
        
        var customers = results.ToList();
        if (customers.Count == 0)
        {
            return "No customers found matching criteria";
        }
        
        return $"Found {customers.Count} customer(s): " + 
               string.Join(", ", customers.Select(c => $"{c.Name} ({c.City})"));
    }

    public async Task<string> DeleteCustomerAsync(int customerId)
    {
        await Task.Delay(50);
        
        if (_customers.Remove(customerId))
        {
            return $"Deleted customer {customerId}";
        }
        return $"Customer {customerId} not found";
    }
}

public record Customer(int Id, string Name, string Email, string City);
