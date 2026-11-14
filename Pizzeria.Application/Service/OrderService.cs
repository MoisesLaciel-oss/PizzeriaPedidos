using Pizzeria.Application.Contract;
using Pizzeria.Application.Core;
using Pizzeria.Application.Dtos.Orders;
using Pizzeria.Domain.Core;
using Pizzeria.Domain.Entities;
using Pizzeria.Infrastructure.Exceptions;
using Pizzeria.Infrastructure.Interfaces;

namespace Pizzeria.Application.Service
{
    public class OrderService : BaseService, IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<ServiceResult> GetAllAsync()
        {
            return Ok(await _orderRepository.GetOrdersAsync());
        }

        public async Task<ServiceResult> GetByIdAsync(int id)
        {
            var order = await _orderRepository.GetOrderAsync(id);
            return order is null ? Fail("The order does not exist.") : Ok(order);
        }

        public async Task<ServiceResult> GetByCustomerAsync(int customerId)
        {
            return Ok(await _orderRepository.GetOrdersByCustomerAsync(customerId));
        }

        public async Task<ServiceResult> SaveAsync(OrderDtoAdd dto)
        {
            var errors = new List<string>();

            ValidateId(errors, dto.CustomerId, "CustomerId");
            ValidateEnum(errors, dto.Type, "Type");
            ValidateLength(errors, dto.DeliveryAddress, "DeliveryAddress", 300);
            ValidateLength(errors, dto.Notes, "Notes", 500);

            if (dto.Details is null || dto.Details.Count == 0)
            {
                errors.Add("The Details field must have at least one product.");
            }
            else
            {
                foreach (var detail in dto.Details)
                    OrderDetailService.ValidateFields(errors, detail);
            }

            if (errors.Count > 0) return Fail(errors.Distinct());

            try
            {
                var order = new Order
                {
                    CustomerId = dto.CustomerId,
                    Type = dto.Type,
                    Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
                    Details = dto.Details!
                        .Select(d => new OrderDetail
                        {
                            ProductId = d.ProductId,
                            SizeId = d.SizeId,
                            Quantity = d.Quantity
                        })
                        .ToList(),
                    Delivery = dto.Type == OrderType.Delivery
                        ? new Delivery { Address = dto.DeliveryAddress?.Trim() ?? string.Empty }
                        : null
                };

                await _orderRepository.SaveAsync(order);

                return Ok(await _orderRepository.GetOrderAsync(order.Id), "Order created successfully.");
            }
            catch (OrderException ex)
            {
                return Fail(ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(OrderDtoUpdate dto)
        {
            var errors = new List<string>();

            ValidateId(errors, dto.Id, "Id");
            ValidateEnum(errors, dto.Status, "Status");
            ValidateLength(errors, dto.Courier, "Courier", 100);

            if (errors.Count > 0) return Fail(errors);

            try
            {
                await _orderRepository.ChangeStatusAsync(dto.Id, dto.Status, dto.Courier);

                return Ok(await _orderRepository.GetOrderAsync(dto.Id), "Order status updated successfully.");
            }
            catch (OrderException ex)
            {
                return Fail(ex.Message);
            }
        }

        public async Task<ServiceResult> RemoveAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order is null) return Fail("The order does not exist.");

            await _orderRepository.RemoveAsync(order);

            return Ok(message: "Order deleted successfully.");
        }
    }
}
