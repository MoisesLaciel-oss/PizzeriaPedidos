using Pizzeria.Domain.Core;

namespace Pizzeria.Domain.Entities
{
    public class ProductIngredient : BaseEntity
    {
        public int ProductId { get; set; }
        public int IngredientId { get; set; }

        public Product Product { get; set; } = null!;
        public Ingredient Ingredient { get; set; } = null!;
    }
}
