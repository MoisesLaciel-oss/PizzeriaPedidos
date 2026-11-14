using Pizzeria.Domain.Core;

namespace Pizzeria.Application.Dtos.Products
{
    public class ProductDtoAdd : DtoBase
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProductCategory Category { get; set; } = ProductCategory.Pizza;
        public List<PriceDto> Prices { get; set; } = new();
        public List<int> IngredientIds { get; set; } = new();
    }
}
