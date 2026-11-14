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

    public class ProductAddModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProductCategory Category { get; set; } = ProductCategory.Pizza;
        public List<PriceAddModel> Prices { get; set; } = new();
        public List<int> IngredientIds { get; set; } = new();
    }

    public class ProductUpdateModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProductCategory Category { get; set; } = ProductCategory.Pizza;
        public bool IsActive { get; set; } = true;
    }

    public class PriceAddModel
    {
        public int SizeId { get; set; }
        public decimal Price { get; set; }
    }
}
