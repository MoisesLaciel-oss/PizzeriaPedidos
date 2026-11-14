using Microsoft.EntityFrameworkCore;
using Pizzeria.Domain.Core;
using Pizzeria.Domain.Entities;
using Pizzeria.Infrastructure.Context;
using Pizzeria.Infrastructure.Core;
using Pizzeria.Infrastructure.Exceptions;
using Pizzeria.Infrastructure.Interfaces;
using Pizzeria.Infrastructure.Models;

namespace Pizzeria.Infrastructure.Repositories
{
    public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(PizzeriaContext context) : base(context)
        {
        }

        public async Task<List<OrderDetailModel>> GetDetailsByOrderAsync(int orderId)
        {
            var details = await Entities.AsNoTracking()
                .Include(d => d.Product)
                .Include(d => d.Size)
                .Where(d => d.OrderId == orderId && !d.IsDeleted)
                .ToListAsync();

            return details.Select(Map).ToList();
        }

        public override async Task SaveAsync(OrderDetail entity)
        {
            var order = await PendingOrderAsync(entity.OrderId);

            entity.UnitPrice = await CurrentPrice.FindAsync(Context, entity);
            entity.CalculateAmount();
            entity.CreatedDate = DateTime.Now;

            order.Details.Add(entity);
            order.CalculateTotals();
            order.ModifiedDate = DateTime.Now;

            await Context.SaveChangesAsync();
        }

        public override async Task RemoveAsync(OrderDetail entity)
        {
            var order = await PendingOrderAsync(entity.OrderId);
            var detail = order.Details.FirstOrDefault(d => d.Id == entity.Id && !d.IsDeleted);

            if (detail is null)
                throw new OrderException("The detail does not exist in the order.");

            if (order.Details.Count(d => !d.IsDeleted) == 1)
                throw new OrderException("The order cannot be left without products; cancel it instead.");

            detail.IsDeleted = true;
            detail.DeletedDate = DateTime.Now;

            order.CalculateTotals();
            order.ModifiedDate = DateTime.Now;

            await Context.SaveChangesAsync();
        }

        private async Task<Order> PendingOrderAsync(int orderId)
        {
            var order = await Context.Orders
                .Include(o => o.Details)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

            if (order is null)
                throw new OrderException("The order does not exist.");

            if (order.Status != OrderStatus.Pending)
                throw new OrderException("Only the products of a pending order can be modified.");

            return order;
        }

        internal static OrderDetailModel Map(OrderDetail detail)
        {
            return new OrderDetailModel
            {
                Id = detail.Id,
                OrderId = detail.OrderId,
                ProductId = detail.ProductId,
                Product = detail.Product.Name,
                SizeId = detail.SizeId,
                Size = detail.Size.Name,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                Amount = detail.Amount
            };
        }
    }
}
