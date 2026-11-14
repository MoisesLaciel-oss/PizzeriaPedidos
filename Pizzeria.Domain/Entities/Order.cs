using Pizzeria.Domain.Core;

namespace Pizzeria.Domain.Entities
{
    public class Order : BaseEntity
    {
        public const decimal TaxRate = 0.18m;
        public const decimal StandardDeliveryFee = 100m;

        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderType Type { get; set; } = OrderType.DineIn;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string? Notes { get; set; }

        public Customer Customer { get; set; } = null!;
        public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
        public Delivery? Delivery { get; set; }

        public void CalculateTotals()
        {
            Subtotal = Details.Where(d => !d.IsDeleted).Sum(d => d.Amount);
            Tax = Math.Round((Subtotal + DeliveryFee) * TaxRate, 2, MidpointRounding.AwayFromZero);
            Total = Subtotal + DeliveryFee + Tax;
        }
    }
}
