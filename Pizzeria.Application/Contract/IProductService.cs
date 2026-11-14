using Pizzeria.Application.Core;
using Pizzeria.Application.Dtos.Products;

namespace Pizzeria.Application.Contract
{
    public interface IProductService : IBaseService<ProductDtoAdd, ProductDtoUpdate>
    {
        Task<ServiceResult> SetPriceAsync(int productId, PriceDto dto);
    }
}
