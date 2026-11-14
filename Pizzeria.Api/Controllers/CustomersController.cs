using Microsoft.AspNetCore.Mvc;
using Pizzeria.Domain.Entities;
using Pizzeria.Infrastructure.Exceptions;
using Pizzeria.Infrastructure.Interfaces;
using Pizzeria.Infrastructure.Models;

namespace Pizzeria.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;

        public CustomersController(ICustomerRepository customerRepository, IOrderRepository orderRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _customerRepository.GetCustomersAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var customer = await _customerRepository.GetCustomerAsync(id);
            return customer is null ? NotFound() : Ok(customer);
        }

        [HttpGet("{id:int}/orders")]
        public async Task<IActionResult> GetOrders(int id)
        {
            if (await _customerRepository.GetCustomerAsync(id) is null) return NotFound();
            return Ok(await _orderRepository.GetOrdersByCustomerAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CustomerAddModel model)
        {
            try
            {
                var customer = new Customer();
                Assign(customer, model);

                await _customerRepository.SaveAsync(customer);

                var created = await _customerRepository.GetCustomerAsync(customer.Id);
                return CreatedAtAction(nameof(Get), new { id = customer.Id }, created);
            }
            catch (CustomerException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] CustomerAddModel model)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer is null) return NotFound();

                Assign(customer, model);

                await _customerRepository.UpdateAsync(customer);
                return Ok(await _customerRepository.GetCustomerAsync(id));
            }
            catch (CustomerException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer is null) return NotFound();

                await _customerRepository.RemoveAsync(customer);
                return NoContent();
            }
            catch (CustomerException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private static void Assign(Customer customer, CustomerAddModel model)
        {
            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Phone = model.Phone;
            customer.Email = model.Email;
            customer.Address = model.Address;
            customer.AddressReference = model.AddressReference;
        }
    }
}
