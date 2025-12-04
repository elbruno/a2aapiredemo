using CartEntities;

namespace Store.Services;

public interface ICheckoutService
{
    // DEMO: Apply agentic checkout with discount computation
    Task<Cart> ApplyAgentCheckoutAsync(Cart cart);
    Task<Order> ProcessOrderAsync(Customer customer, Cart cart);
    Task<Order?> GetOrderAsync(string orderNumber);
}