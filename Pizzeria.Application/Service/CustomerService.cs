using System.Text.RegularExpressions;
using Pizzeria.Application.Contract;
using Pizzeria.Application.Core;
using Pizzeria.Application.Dtos.Customers;
using Pizzeria.Domain.Entities;
using Pizzeria.Infrastructure.Exceptions;
using Pizzeria.Infrastructure.Interfaces;

namespace Pizzeria.Application.Service
{
    public class CustomerService : BaseService, ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<ServiceResult> GetAllAsync()
        {
            return Ok(await _customerRepository.GetCustomersAsync());
        }

        public async Task<ServiceResult> GetByIdAsync(int id)
        {
            var customer = await _customerRepository.GetCustomerAsync(id);
            return customer is null ? Fail("The customer does not exist.") : Ok(customer);
        }

        public async Task<ServiceResult> SaveAsync(CustomerDtoAdd dto)
        {
            var errors = Validate(dto);
            if (errors.Count > 0) return Fail(errors);

            try
            {
                var customer = new Customer();
                Assign(customer, dto);

                await _customerRepository.SaveAsync(customer);

                return Ok(await _customerRepository.GetCustomerAsync(customer.Id), "Customer created successfully.");
            }
            catch (CustomerException ex)
            {
                return Fail(ex.Message);
            }
        }

        public async Task<ServiceResult> UpdateAsync(CustomerDtoUpdate dto)
        {
            var errors = Validate(dto);
            ValidateId(errors, dto.Id, "Id");
            if (errors.Count > 0) return Fail(errors);

            try
            {
                var customer = await _customerRepository.GetByIdAsync(dto.Id);
                if (customer is null) return Fail("The customer does not exist.");

                Assign(customer, dto);

                await _customerRepository.UpdateAsync(customer);

                return Ok(await _customerRepository.GetCustomerAsync(customer.Id), "Customer updated successfully.");
            }
            catch (CustomerException ex)
            {
                return Fail(ex.Message);
            }
        }

        public async Task<ServiceResult> RemoveAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer is null) return Fail("The customer does not exist.");

                await _customerRepository.RemoveAsync(customer);

                return Ok(message: "Customer deleted successfully.");
            }
            catch (CustomerException ex)
            {
                return Fail(ex.Message);
            }
        }

        private static List<string> Validate(CustomerDtoAdd dto)
        {
            var errors = new List<string>();

            ValidateRequired(errors, dto.FirstName, "FirstName", 100);
            ValidateRequired(errors, dto.LastName, "LastName", 100);
            ValidateRequired(errors, dto.Phone, "Phone", 20);
            ValidateLength(errors, dto.Email, "Email", 150);
            ValidateLength(errors, dto.Address, "Address", 300);
            ValidateLength(errors, dto.AddressReference, "AddressReference", 300);

            if (!string.IsNullOrWhiteSpace(dto.Phone) && dto.Phone.Count(char.IsDigit) != 10)
                errors.Add("The Phone field must contain 10 digits.");

            if (!string.IsNullOrWhiteSpace(dto.Email) &&
                !Regex.IsMatch(dto.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("The Email field is not a valid email address.");

            return errors;
        }

        private static void Assign(Customer customer, CustomerDtoAdd dto)
        {
            customer.FirstName = dto.FirstName.Trim();
            customer.LastName = dto.LastName.Trim();
            customer.Phone = dto.Phone.Trim();
            customer.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
            customer.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
            customer.AddressReference = string.IsNullOrWhiteSpace(dto.AddressReference) ? null : dto.AddressReference.Trim();
        }
    }
}
