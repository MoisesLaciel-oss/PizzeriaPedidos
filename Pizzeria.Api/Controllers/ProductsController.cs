using Microsoft.AspNetCore.Mvc;
using Pizzeria.Application.Contract;
using Pizzeria.Application.Dtos.Products;

namespace Pizzeria.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _productService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductDtoAdd dto)
        {
            var result = await _productService.SaveAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductDtoUpdate dto)
        {
            dto.Id = id;

            var result = await _productService.UpdateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}/prices")]
        public async Task<IActionResult> PutPrice(int id, [FromBody] PriceDto dto)
        {
            var result = await _productService.SetPriceAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.RemoveAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
