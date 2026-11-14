using Pizzeria.Domain.Core;

namespace Pizzeria.Domain.Entities
{
    public class Size : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int? Inches { get; set; }
        public int? Slices { get; set; }

        public ICollection<ProductPrice> Prices { get; set; } = new List<ProductPrice>();
    }
}
