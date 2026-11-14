using Pizzeria.Domain.Entities;
using Pizzeria.Domain.Repository;
using Pizzeria.Infrastructure.Models;

namespace Pizzeria.Infrastructure.Interfaces
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<List<CustomerModel>> GetCustomersAsync();
        Task<CustomerModel?> GetCustomerAsync(int id);
    }
}
