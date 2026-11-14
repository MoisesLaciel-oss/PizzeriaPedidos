using Pizzeria.Application.Core;
using Pizzeria.Application.Dtos.Customers;

namespace Pizzeria.Application.Contract
{
    public interface ICustomerService : IBaseService<CustomerDtoAdd, CustomerDtoUpdate>
    {
    }
}
