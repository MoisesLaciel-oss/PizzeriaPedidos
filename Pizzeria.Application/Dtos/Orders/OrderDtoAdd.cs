using Pizzeria.Application.Dtos.OrderDetails;
using Pizzeria.Domain.Core;

namespace Pizzeria.Application.Dtos.Orders
{
    public class OrderDtoAdd : DtoBase
    {
        public int CustomerId { get; set; }
        public OrderType Type { get; set; } = OrderType.DineIn;
        public string? DeliveryAddress { get; set; }
        public string? Notes { get; set; }
        public List<OrderDetailDtoAdd> Details { get; set; } = new();
    }
}
