using Pizzeria.Domain.Core;

namespace Pizzeria.Domain.Entities
{
    public class Ingredient : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal ExtraPrice { get; set; }
        public bool IsAvailable { get; set; } = true;

        public ICollection<ProductIngredient> Products { get; set; } = new List<ProductIngredient>();
    }
}
