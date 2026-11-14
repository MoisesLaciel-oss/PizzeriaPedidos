using Pizzeria.Application.Contract;
using Pizzeria.Application.Core;
using Pizzeria.Application.Dtos.Products;
using Pizzeria.Domain.Entities;
using Pizzeria.Infrastructure.Exceptions;
using Pizzeria.Infrastructure.Interfaces;

namespace Pizzeria.Application.Service
{
    public class ProductService : BaseService, IProductService
    {
        private const decimal MaxPrice = 99999999.99m;

        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ServiceResult> GetAllAsync()
        {
            return Ok(await _productRepository.GetProductsAsync());
        }

        public async Task<ServiceResult> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetProductAsync(id);
            return product is null ? Fail("The product does not exist.") : Ok(product);
        }

        public async Task<ServiceResult> SaveAsync(ProductDtoAdd dto)
        {
            var errors = new List<string>();

            ValidateRequired(errors, dto.Name, "Name", 100);
            ValidateLength(errors, dto.Description, "Description", 300);
            ValidateEnum(errors, dto.Category, "Category");

            if (dto.Prices is null || dto.Prices.Count == 0)
            {
                errors.Add("The Prices field must have at least one price.");
            }
            else
            {
                foreach (var price in dto.Prices)
                    ValidatePrice(errors, price);

                if (dto.Prices.GroupBy(p => p.SizeId).Any(g => g.Count() > 1))
                    errors.Add("The same size cannot be repeated in the Prices field.");
            }

            if (dto.IngredientIds is not null && dto.IngredientIds.Any(id => id <= 0))
                errors.Add("The IngredientIds field must contain values greater than zero.");

            if (errors.Count > 0) return Fail(errors);

            try
            {
                var product = new Product
                {
                    Name = dto.Name.Trim(),
                    Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                    Category = dto.Category,
                    IsActive = true,
                    Prices = dto.Prices!
                        .Select(p => new ProductPrice { SizeId = p.SizeId, Price = p.Price })
                        .ToList(),
                    Ingredients = (dto.IngredientIds ?? new List<int>())
                        .Distinct()
                        .Select(id => new ProductIngredient { IngredientId = id })
                        .ToList()
                };

                await _productRepository.SaveAsync(product);

                return Ok(await _productRepository.GetProductAsync(product.Id), "Product created successfully.");
            }
            catch (ProductException ex)
            {
                return Fail(ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(ProductDtoUpdate dto)
        {
            var errors = new List<string>();

            ValidateId(errors, dto.Id, "Id");
            ValidateRequired(errors, dto.Name, "Name", 100);
            ValidateLength(errors, dto.Description, "Description", 300);
            ValidateEnum(errors, dto.Category, "Category");

            if (errors.Count > 0) return Fail(errors);

            try
            {
                var product = await _productRepository.GetByIdAsync(dto.Id);
                if (product is null) return Fail("The product does not exist.");

                product.Name = dto.Name.Trim();
                product.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
                product.Category = dto.Category;
                product.IsActive = dto.IsActive;

                await _productRepository.UpdateAsync(product);

                return Ok(await _productRepository.GetProductAsync(product.Id), "Product updated successfully.");
            }
            catch (ProductException ex)
            {
                return Fail(ex.Message);
            }
        }

        public async Task<ServiceResult> RemoveAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null) return Fail("The product does not exist.");

            await _productRepository.RemoveAsync(product);

            return Ok(message: "Product deleted successfully.");
        }

        public async Task<ServiceResult> SetPriceAsync(int productId, PriceDto dto)
        {
            var errors = new List<string>();

            ValidateId(errors, productId, "ProductId");
            ValidatePrice(errors, dto);

            if (errors.Count > 0) return Fail(errors);

            try
            {
                await _productRepository.SetPriceAsync(productId, dto.SizeId, dto.Price);

                return Ok(await _productRepository.GetProductAsync(productId), "Price saved successfully.");
            }
            catch (ProductException ex)
            {
                return Fail(ex.Message);
            }
        }

        private static void ValidatePrice(List<string> errors, PriceDto price)
        {
            ValidateId(errors, price.SizeId, "SizeId");

            if (price.Price <= 0)
                errors.Add("The Price field must be greater than zero.");
            else if (price.Price > MaxPrice)
                errors.Add($"The Price field cannot exceed {MaxPrice}.");
        }
    }
}
