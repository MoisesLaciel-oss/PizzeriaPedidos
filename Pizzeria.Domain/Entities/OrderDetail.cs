using Pizzeria.Domain.Core;

namespace Pizzeria.Domain.Entities
{
    public class OrderDetail : BaseEntity
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }

        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public Size Size { get; set; } = null!;

        public void CalculateAmount()
        {
            Amount = Math.Round(UnitPrice * Quantity, 2, MidpointRounding.AwayFromZero);
        }
    }
}
