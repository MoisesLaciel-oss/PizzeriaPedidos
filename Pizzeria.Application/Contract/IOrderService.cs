using Pizzeria.Application.Core;
using Pizzeria.Application.Dtos.Orders;

namespace Pizzeria.Application.Contract
{
    public interface IOrderService : IBaseService<OrderDtoAdd, OrderDtoUpdate>
    {
        Task<ServiceResult> GetByCustomerAsync(int customerId);
    }
}
