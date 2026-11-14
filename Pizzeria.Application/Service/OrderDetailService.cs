using Pizzeria.Application.Contract;
using Pizzeria.Application.Core;
using Pizzeria.Application.Dtos.OrderDetails;
using Pizzeria.Domain.Entities;
using Pizzeria.Infrastructure.Exceptions;
using Pizzeria.Infrastructure.Interfaces;

namespace Pizzeria.Application.Service
{
    public class OrderDetailService : BaseService, IOrderDetailService
    {
        private const int MaxQuantity = 100;

        private readonly IOrderDetailRepository _detailRepository;

        public OrderDetailService(IOrderDetailRepository detailRepository)
        {
            _detailRepository = detailRepository;
        }

        public async Task<ServiceResult> GetByOrderAsync(int orderId)
        {
            return Ok(await _detailRepository.GetDetailsByOrderAsync(orderId));
        }

        public async Task<ServiceResult> SaveAsync(OrderDetailDtoAdd dto)
        {
            var errors = new List<string>();

            ValidateId(errors, dto.OrderId, "OrderId");
            ValidateFields(errors, dto);

            if (errors.Count > 0) return Fail(errors);

            try
            {
                await _detailRepository.SaveAsync(new OrderDetail
                {
                    OrderId = dto.OrderId,
                    ProductId = dto.ProductId,
                    SizeId = dto.SizeId,
                    Quantity = dto.Quantity
                });

                return Ok(await _detailRepository.GetDetailsByOrderAsync(dto.OrderId), "Detail added successfully.");
            }
            catch (OrderException ex)
            {
                return Fail(ex.Message);
            }
        }

        public async Task<ServiceResult> RemoveAsync(int orderId, int detailId)
        {
            var errors = new List<string>();

            ValidateId(errors, orderId, "OrderId");
            ValidateId(errors, detailId, "DetailId");

            if (errors.Count > 0) return Fail(errors);

            try
            {
                await _detailRepository.RemoveAsync(new OrderDetail { Id = detailId, OrderId = orderId });

                return Ok(await _detailRepository.GetDetailsByOrderAsync(orderId), "Detail deleted successfully.");
            }
            catch (OrderException ex)
            {
                return Fail(ex.Message);
            }
        }

        internal static void ValidateFields(List<string> errors, OrderDetailDtoAdd dto)
        {
            ValidateId(errors, dto.ProductId, "ProductId");
            ValidateId(errors, dto.SizeId, "SizeId");

            if (dto.Quantity <= 0)
                errors.Add("The Quantity field must be greater than zero.");
            else if (dto.Quantity > MaxQuantity)
                errors.Add($"The Quantity field cannot exceed {MaxQuantity}.");
        }
    }
}
