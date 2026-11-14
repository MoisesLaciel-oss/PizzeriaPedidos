using System.Text.Json.Serialization;
using Pizzeria.Domain.Core;

namespace Pizzeria.Application.Dtos.Products
{
    public class ProductDtoUpdate : DtoBase
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProductCategory Category { get; set; } = ProductCategory.Pizza;
        public bool IsActive { get; set; } = true;
    }
}
