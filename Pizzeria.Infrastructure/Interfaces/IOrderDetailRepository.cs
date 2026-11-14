using Pizzeria.Domain.Entities;
using Pizzeria.Domain.Repository;
using Pizzeria.Infrastructure.Models;

namespace Pizzeria.Infrastructure.Interfaces
{
    public interface IOrderDetailRepository : IBaseRepository<OrderDetail>
    {
        Task<List<OrderDetailModel>> GetDetailsByOrderAsync(int orderId);
    }
}
