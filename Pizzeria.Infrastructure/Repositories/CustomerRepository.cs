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
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(PizzeriaContext context) : base(context)
        {
        }

        public async Task<List<CustomerModel>> GetCustomersAsync()
        {
            var customers = await Entities.AsNoTracking()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToListAsync();

            return customers.Select(Map).ToList();
        }

        public async Task<CustomerModel?> GetCustomerAsync(int id)
        {
            var customer = await Entities.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            return customer is null ? null : Map(customer);
        }

        public override async Task SaveAsync(Customer entity)
        {
            await ValidateAsync(entity);
            await base.SaveAsync(entity);
        }

        public override async Task UpdateAsync(Customer entity)
        {
            await ValidateAsync(entity);
            await base.UpdateAsync(entity);
        }

        public override async Task RemoveAsync(Customer entity)
        {
            var hasOpenOrders = await Context.Orders.AnyAsync(o =>
                o.CustomerId == entity.Id && !o.IsDeleted &&
                o.Status != OrderStatus.Delivered &&
                o.Status != OrderStatus.Cancelled);

            if (hasOpenOrders)
                throw new CustomerException("A customer with orders in progress cannot be deleted.");

            await base.RemoveAsync(entity);
        }

        private async Task ValidateAsync(Customer entity)
        {
            if (string.IsNullOrWhiteSpace(entity.FirstName))
                throw new CustomerException("The customer first name is required.");

            if (string.IsNullOrWhiteSpace(entity.Phone))
                throw new CustomerException("The customer phone is required.");

            entity.FirstName = entity.FirstName.Trim();
            entity.LastName = (entity.LastName ?? string.Empty).Trim();
            entity.Phone = entity.Phone.Trim();

            if (await ExistsAsync(c => c.Phone == entity.Phone && c.Id != entity.Id))
                throw new CustomerException($"A customer with phone {entity.Phone} already exists.");
        }

        private static CustomerModel Map(Customer customer)
        {
            return new CustomerModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                AddressReference = customer.AddressReference,
                CreatedDate = customer.CreatedDate
            };
        }
    }
}
