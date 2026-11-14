using Microsoft.AspNetCore.Mvc;
using Pizzeria.Application.Contract;
using Pizzeria.Application.Dtos.Customers;

namespace Pizzeria.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;

        public CustomersController(ICustomerService customerService, IOrderService orderService)
        {
            _customerService = customerService;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _customerService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{id:int}/orders")]
        public async Task<IActionResult> GetOrders(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (!customer.Success) return NotFound(customer);

            return Ok(await _orderService.GetByCustomerAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerDtoAdd dto)
        {
            var result = await _customerService.SaveAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] CustomerDtoUpdate dto)
        {
            dto.Id = id;

            var result = await _customerService.UpdateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.RemoveAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
