using Pizzeria.Domain.Core;

namespace Pizzeria.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProductCategory Category { get; set; } = ProductCategory.Pizza;
        public bool IsActive { get; set; } = true;

        public ICollection<ProductPrice> Prices { get; set; } = new List<ProductPrice>();
        public ICollection<ProductIngredient> Ingredients { get; set; } = new List<ProductIngredient>();
    }
}
