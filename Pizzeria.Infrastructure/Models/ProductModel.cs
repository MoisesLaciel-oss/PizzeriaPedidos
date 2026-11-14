using Pizzeria.Domain.Core;

namespace Pizzeria.Infrastructure.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProductCategory Category { get; set; }
        public bool IsActive { get; set; }
        public List<PriceModel> Prices { get; set; } = new();
        public List<string> Ingredients { get; set; } = new();
    }

    public class PriceModel
    {
        public int SizeId { get; set; }
        public string Size { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
