using Microsoft.AspNetCore.Mvc;
using Pizzeria.Application.Contract;
using Pizzeria.Application.Dtos.OrderDetails;
using Pizzeria.Application.Dtos.Orders;

namespace Pizzeria.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _detailService;

        public OrdersController(IOrderService orderService, IOrderDetailService detailService)
        {
            _orderService = orderService;
            _detailService = detailService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _orderService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _orderService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderDtoAdd dto)
        {
            var result = await _orderService.SaveAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> PatchStatus(int id, [FromBody] OrderDtoUpdate dto)
        {
            dto.Id = id;

            var result = await _orderService.UpdateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _orderService.RemoveAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (!order.Success) return NotFound(order);

            return Ok(await _detailService.GetByOrderAsync(id));
        }

        [HttpPost("{id:int}/details")]
        public async Task<IActionResult> PostDetail(int id, [FromBody] OrderDetailDtoAdd dto)
        {
            dto.OrderId = id;

            var result = await _detailService.SaveAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}/details/{detailId:int}")]
        public async Task<IActionResult> DeleteDetail(int id, int detailId)
        {
            var result = await _detailService.RemoveAsync(id, detailId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
