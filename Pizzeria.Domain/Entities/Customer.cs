using Pizzeria.Domain.Core;

namespace Pizzeria.Domain.Entities
{
    public class Customer : Person
    {
        public string? Address { get; set; }
        public string? AddressReference { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
