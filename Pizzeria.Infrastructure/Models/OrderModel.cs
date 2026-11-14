using Pizzeria.Domain.Core;

namespace Pizzeria.Infrastructure.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string Customer { get; set; } = string.Empty;
        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string? Notes { get; set; }
        public List<OrderDetailModel> Details { get; set; } = new();
        public DeliveryModel? Delivery { get; set; }
    }

    public class DeliveryModel
    {
        public string Address { get; set; } = string.Empty;
        public string? Courier { get; set; }
        public DeliveryStatus Status { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
    }
}
