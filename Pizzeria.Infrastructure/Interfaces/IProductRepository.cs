using Pizzeria.Domain.Entities;
using Pizzeria.Domain.Repository;
using Pizzeria.Infrastructure.Models;

namespace Pizzeria.Infrastructure.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<List<ProductModel>> GetProductsAsync();
        Task<ProductModel?> GetProductAsync(int id);
        Task SetPriceAsync(int productId, int sizeId, decimal price);
    }
}
