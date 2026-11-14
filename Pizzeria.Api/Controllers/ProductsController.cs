using Microsoft.AspNetCore.Mvc;
using Pizzeria.Domain.Entities;
using Pizzeria.Infrastructure.Exceptions;
using Pizzeria.Infrastructure.Interfaces;
using Pizzeria.Infrastructure.Models;

namespace Pizzeria.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _productRepository.GetProductsAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productRepository.GetProductAsync(id);
            return product is null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductAddModel model)
        {
            try
            {
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Category = model.Category,
                    IsActive = true,
                    Prices = model.Prices
                        .Select(p => new ProductPrice { SizeId = p.SizeId, Price = p.Price })
                        .ToList(),
                    Ingredients = model.IngredientIds
                        .Distinct()
                        .Select(id => new ProductIngredient { IngredientId = id })
                        .ToList()
                };

                await _productRepository.SaveAsync(product);

                var created = await _productRepository.GetProductAsync(product.Id);
                return CreatedAtAction(nameof(Get), new { id = product.Id }, created);
            }
            catch (ProductException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductUpdateModel model)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product is null) return NotFound();

                product.Name = model.Name;
                product.Description = model.Description;
                product.Category = model.Category;
                product.IsActive = model.IsActive;

                await _productRepository.UpdateAsync(product);
                return Ok(await _productRepository.GetProductAsync(id));
            }
            catch (ProductException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}/prices")]
        public async Task<IActionResult> PutPrice(int id, [FromBody] PriceAddModel model)
        {
            try
            {
                await _productRepository.SetPriceAsync(id, model.SizeId, model.Price);
                return Ok(await _productRepository.GetProductAsync(id));
            }
            catch (ProductException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null) return NotFound();

            await _productRepository.RemoveAsync(product);
            return NoContent();
        }
    }
}
