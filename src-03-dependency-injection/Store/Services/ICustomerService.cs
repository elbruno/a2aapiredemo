using DataEntities;

namespace Store.Services;

public interface ICustomerService
{
    Customer GetCurrentCustomer();
    void SetCurrentCustomer(int customerId);
    List<Customer> GetAllCustomers();
}
