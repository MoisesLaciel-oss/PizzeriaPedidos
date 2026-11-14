using Pizzeria.Application.Core;
using Pizzeria.Application.Dtos.OrderDetails;

namespace Pizzeria.Application.Contract
{
    public interface IOrderDetailService
    {
        Task<ServiceResult> GetByOrderAsync(int orderId);
        Task<ServiceResult> SaveAsync(OrderDetailDtoAdd dto);
        Task<ServiceResult> RemoveAsync(int orderId, int detailId);
    }
}
