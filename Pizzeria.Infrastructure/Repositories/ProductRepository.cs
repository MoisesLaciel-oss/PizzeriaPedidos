using Microsoft.EntityFrameworkCore;
using Pizzeria.Domain.Entities;
using Pizzeria.Infrastructure.Context;
using Pizzeria.Infrastructure.Core;
using Pizzeria.Infrastructure.Exceptions;
using Pizzeria.Infrastructure.Interfaces;
using Pizzeria.Infrastructure.Models;

namespace Pizzeria.Infrastructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(PizzeriaContext context) : base(context)
        {
        }

        public async Task<List<ProductModel>> GetProductsAsync()
        {
            var products = await WithDetails()
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToListAsync();

            return products.Select(Map).ToList();
        }

        public async Task<ProductModel?> GetProductAsync(int id)
        {
            var product = await WithDetails().FirstOrDefaultAsync(p => p.Id == id);
            return product is null ? null : Map(product);
        }

        public override async Task SaveAsync(Product entity)
        {
            await ValidateAsync(entity);

            if (entity.Prices.Count == 0)
                throw new ProductException("The product must have at least one price.");

            if (entity.Prices.GroupBy(p => p.SizeId).Any(g => g.Count() > 1))
                throw new ProductException("The same size cannot be repeated in the prices.");

            foreach (var price in entity.Prices)
            {
                if (price.Price <= 0)
                    throw new ProductException("The price must be greater than zero.");

                if (!await Context.Sizes.AnyAsync(s => s.Id == price.SizeId && !s.IsDeleted))
                    throw new ProductException($"Size {price.SizeId} does not exist.");

                price.CreatedDate = DateTime.Now;
            }

            foreach (var ingredient in entity.Ingredients)
            {
                if (!await Context.Ingredients.AnyAsync(i => i.Id == ingredient.IngredientId && !i.IsDeleted))
                    throw new ProductException($"Ingredient {ingredient.IngredientId} does not exist.");

                ingredient.CreatedDate = DateTime.Now;
            }

            await base.SaveAsync(entity);
        }

        public override async Task UpdateAsync(Product entity)
        {
            await ValidateAsync(entity);
            await base.UpdateAsync(entity);
        }

        public async Task SetPriceAsync(int productId, int sizeId, decimal price)
        {
            if (price <= 0)
                throw new ProductException("The price must be greater than zero.");

            if (!await ExistsAsync(p => p.Id == productId))
                throw new ProductException("The product does not exist.");

            if (!await Context.Sizes.AnyAsync(s => s.Id == sizeId && !s.IsDeleted))
                throw new ProductException("The size does not exist.");

            var current = await Context.ProductPrices
                .FirstOrDefaultAsync(pp => pp.ProductId == productId && pp.SizeId == sizeId && !pp.IsDeleted);

            if (current is null)
            {
                await Context.ProductPrices.AddAsync(new ProductPrice
                {
                    ProductId = productId,
                    SizeId = sizeId,
                    Price = price,
                    CreatedDate = DateTime.Now
                });
            }
            else
            {
                current.Price = price;
                current.ModifiedDate = DateTime.Now;
            }

            await Context.SaveChangesAsync();
        }

        private IQueryable<Product> WithDetails()
        {
            return Entities.AsNoTracking()
                .Include(p => p.Prices).ThenInclude(pp => pp.Size)
                .Include(p => p.Ingredients).ThenInclude(pi => pi.Ingredient)
                .Where(p => !p.IsDeleted);
        }

        private async Task ValidateAsync(Product entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ProductException("The product name is required.");

            entity.Name = entity.Name.Trim();

            if (await ExistsAsync(p => p.Name == entity.Name && p.Id != entity.Id))
                throw new ProductException($"A product named '{entity.Name}' already exists.");
        }

        private static ProductModel Map(Product product)
        {
            return new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                IsActive = product.IsActive,
                Prices = product.Prices
                    .Where(pp => !pp.IsDeleted)
                    .OrderBy(pp => pp.Price)
                    .Select(pp => new PriceModel
                    {
                        SizeId = pp.SizeId,
                        Size = pp.Size.Name,
                        Price = pp.Price
                    })
                    .ToList(),
                Ingredients = product.Ingredients
                    .Where(pi => !pi.IsDeleted)
                    .Select(pi => pi.Ingredient.Name)
                    .OrderBy(name => name)
                    .ToList()
            };
        }
    }
}
