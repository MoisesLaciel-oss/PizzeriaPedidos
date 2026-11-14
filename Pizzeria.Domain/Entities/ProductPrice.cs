using Pizzeria.Domain.Core;

namespace Pizzeria.Domain.Entities
{
    public class ProductPrice : BaseEntity
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public decimal Price { get; set; }

        public Product Product { get; set; } = null!;
        public Size Size { get; set; } = null!;
    }
}
