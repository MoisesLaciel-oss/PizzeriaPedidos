namespace Pizzeria.Application.Dtos.Customers
{
    public class CustomerDtoAdd : DtoBase
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? AddressReference { get; set; }
    }
}
