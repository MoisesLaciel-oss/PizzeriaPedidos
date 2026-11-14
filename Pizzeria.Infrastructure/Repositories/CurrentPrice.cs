using Microsoft.EntityFrameworkCore;
using Pizzeria.Domain.Entities;
using Pizzeria.Infrastructure.Context;
using Pizzeria.Infrastructure.Exceptions;

namespace Pizzeria.Infrastructure.Repositories
{
    internal static class CurrentPrice
    {
        public static async Task<decimal> FindAsync(PizzeriaContext context, OrderDetail detail)
        {
            if (detail.Quantity <= 0)
                throw new OrderException("The quantity of each product must be greater than zero.");

            var price = await context.ProductPrices
                .AsNoTracking()
                .Include(pp => pp.Product)
                .FirstOrDefaultAsync(pp =>
                    pp.ProductId == detail.ProductId &&
                    pp.SizeId == detail.SizeId &&
                    !pp.IsDeleted);

            if (price is null || price.Product.IsDeleted)
                throw new OrderException(
                    $"Product {detail.ProductId} has no price for size {detail.SizeId}.");

            if (!price.Product.IsActive)
                throw new OrderException($"The product '{price.Product.Name}' is not available.");

            return price.Price;
        }
    }
}
