using Pizzeria.Domain.Core;
using Pizzeria.Domain.Entities;
using Pizzeria.Domain.Repository;
using Pizzeria.Infrastructure.Models;

namespace Pizzeria.Infrastructure.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<List<OrderModel>> GetOrdersAsync();
        Task<List<OrderModel>> GetOrdersByCustomerAsync(int customerId);
        Task<OrderModel?> GetOrderAsync(int id);
        Task ChangeStatusAsync(int orderId, OrderStatus newStatus, string? courier = null);
    }
}
