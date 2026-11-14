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
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(PizzeriaContext context) : base(context)
        {
        }

        public async Task<List<OrderModel>> GetOrdersAsync()
        {
            var orders = await WithDetails().OrderByDescending(o => o.OrderDate).ToListAsync();
            return orders.Select(Map).ToList();
        }

        public async Task<List<OrderModel>> GetOrdersByCustomerAsync(int customerId)
        {
            var orders = await WithDetails()
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(Map).ToList();
        }

        public async Task<OrderModel?> GetOrderAsync(int id)
        {
            var order = await WithDetails().FirstOrDefaultAsync(o => o.Id == id);
            return order is null ? null : Map(order);
        }

        public override async Task SaveAsync(Order entity)
        {
            var customer = await Context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == entity.CustomerId && !c.IsDeleted);

            if (customer is null)
                throw new OrderException("The order customer does not exist.");

            if (entity.Details.Count == 0)
                throw new OrderException("The order must have at least one product.");

            var now = DateTime.Now;

            foreach (var detail in entity.Details)
            {
                detail.UnitPrice = await CurrentPrice.FindAsync(Context, detail);
                detail.CalculateAmount();
                detail.CreatedDate = now;
            }

            if (entity.Type == OrderType.Delivery)
            {
                entity.Delivery ??= new Delivery();

                if (string.IsNullOrWhiteSpace(entity.Delivery.Address))
                    entity.Delivery.Address = customer.Address ?? string.Empty;

                if (string.IsNullOrWhiteSpace(entity.Delivery.Address))
                    throw new OrderException("A delivery order requires a delivery address.");

                entity.Delivery.Status = DeliveryStatus.Pending;
                entity.Delivery.CreatedDate = now;
                entity.DeliveryFee = Order.StandardDeliveryFee;
            }
            else
            {
                entity.Delivery = null;
                entity.DeliveryFee = 0m;
            }

            entity.OrderDate = now;
            entity.Status = OrderStatus.Pending;
            entity.CalculateTotals();

            await base.SaveAsync(entity);
        }

        public async Task ChangeStatusAsync(int orderId, OrderStatus newStatus, string? courier = null)
        {
            var order = await Entities
                .Include(o => o.Delivery)
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

            if (order is null)
                throw new OrderException("The order does not exist.");

            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                throw new OrderException($"The order is already {order.Status} and its status cannot be changed.");

            if (newStatus == OrderStatus.OnTheWay && order.Type != OrderType.Delivery)
                throw new OrderException("Only delivery orders can be set to 'OnTheWay'.");

            var now = DateTime.Now;
            order.Status = newStatus;
            order.ModifiedDate = now;

            if (order.Delivery is not null)
            {
                switch (newStatus)
                {
                    case OrderStatus.OnTheWay:
                        order.Delivery.Status = DeliveryStatus.OnTheWay;
                        order.Delivery.DepartureDate = now;
                        if (!string.IsNullOrWhiteSpace(courier))
                            order.Delivery.Courier = courier.Trim();
                        break;
                    case OrderStatus.Delivered:
                        order.Delivery.Status = DeliveryStatus.Delivered;
                        order.Delivery.DepartureDate ??= now;
                        order.Delivery.DeliveredDate = now;
                        break;
                    case OrderStatus.Cancelled:
                        order.Delivery.Status = DeliveryStatus.Cancelled;
                        break;
                }

                order.Delivery.ModifiedDate = now;
            }

            await Context.SaveChangesAsync();
        }

        private IQueryable<Order> WithDetails()
        {
            return Entities.AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Delivery)
                .Include(o => o.Details).ThenInclude(d => d.Product)
                .Include(o => o.Details).ThenInclude(d => d.Size)
                .Where(o => !o.IsDeleted);
        }

        private static OrderModel Map(Order order)
        {
            return new OrderModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                Customer = order.Customer.FullName,
                Type = order.Type,
                Status = order.Status,
                Subtotal = order.Subtotal,
                DeliveryFee = order.DeliveryFee,
                Tax = order.Tax,
                Total = order.Total,
                Notes = order.Notes,
                Details = order.Details
                    .Where(d => !d.IsDeleted)
                    .Select(OrderDetailRepository.Map)
                    .ToList(),
                Delivery = order.Delivery is null || order.Delivery.IsDeleted
                    ? null
                    : new DeliveryModel
                    {
                        Address = order.Delivery.Address,
                        Courier = order.Delivery.Courier,
                        Status = order.Delivery.Status,
                        DepartureDate = order.Delivery.DepartureDate,
                        DeliveredDate = order.Delivery.DeliveredDate
                    }
            };
        }
    }
}
