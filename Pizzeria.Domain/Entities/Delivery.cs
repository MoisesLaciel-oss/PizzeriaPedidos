using Pizzeria.Domain.Core;

namespace Pizzeria.Domain.Entities
{
    public class Delivery : BaseEntity
    {
        public int OrderId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? Courier { get; set; }
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
        public DateTime? DepartureDate { get; set; }
        public DateTime? DeliveredDate { get; set; }

        public Order Order { get; set; } = null!;
    }
}
