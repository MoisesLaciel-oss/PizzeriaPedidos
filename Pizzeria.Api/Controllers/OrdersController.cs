using Microsoft.AspNetCore.Mvc;
using Pizzeria.Domain.Core;
using Pizzeria.Domain.Entities;
using Pizzeria.Infrastructure.Exceptions;
using Pizzeria.Infrastructure.Interfaces;
using Pizzeria.Infrastructure.Models;

namespace Pizzeria.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _detailRepository;

        public OrdersController(IOrderRepository orderRepository, IOrderDetailRepository detailRepository)
        {
            _orderRepository = orderRepository;
            _detailRepository = detailRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _orderRepository.GetOrdersAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _orderRepository.GetOrderAsync(id);
            return order is null ? NotFound() : Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderAddModel model)
        {
            try
            {
                var order = new Order
                {
                    CustomerId = model.CustomerId,
                    Type = model.Type,
                    Notes = model.Notes,
                    Details = model.Details
                        .Select(d => new OrderDetail
                        {
                            ProductId = d.ProductId,
                            SizeId = d.SizeId,
                            Quantity = d.Quantity
                        })
                        .ToList(),
                    Delivery = model.Type == OrderType.Delivery
                        ? new Delivery { Address = model.DeliveryAddress?.Trim() ?? string.Empty }
                        : null
                };

                await _orderRepository.SaveAsync(order);

                var created = await _orderRepository.GetOrderAsync(order.Id);
                return CreatedAtAction(nameof(Get), new { id = order.Id }, created);
            }
            catch (OrderException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> PatchStatus(int id, [FromBody] ChangeStatusModel model)
        {
            try
            {
                if (await _orderRepository.GetOrderAsync(id) is null) return NotFound();

                await _orderRepository.ChangeStatusAsync(id, model.Status, model.Courier);
                return Ok(await _orderRepository.GetOrderAsync(id));
            }
            catch (OrderException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order is null) return NotFound();

            await _orderRepository.RemoveAsync(order);
            return NoContent();
        }

        [HttpGet("{id:int}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            if (await _orderRepository.GetOrderAsync(id) is null) return NotFound();
            return Ok(await _detailRepository.GetDetailsByOrderAsync(id));
        }

        [HttpPost("{id:int}/details")]
        public async Task<IActionResult> PostDetail(int id, [FromBody] OrderDetailAddModel model)
        {
            try
            {
                await _detailRepository.SaveAsync(new OrderDetail
                {
                    OrderId = id,
                    ProductId = model.ProductId,
                    SizeId = model.SizeId,
                    Quantity = model.Quantity
                });

                return Ok(await _orderRepository.GetOrderAsync(id));
            }
            catch (OrderException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}/details/{detailId:int}")]
        public async Task<IActionResult> DeleteDetail(int id, int detailId)
        {
            try
            {
                await _detailRepository.RemoveAsync(new OrderDetail { Id = detailId, OrderId = id });
                return Ok(await _orderRepository.GetOrderAsync(id));
            }
            catch (OrderException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
