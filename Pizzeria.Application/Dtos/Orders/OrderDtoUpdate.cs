using System.Text.Json.Serialization;
using Pizzeria.Domain.Core;

namespace Pizzeria.Application.Dtos.Orders
{
    public class OrderDtoUpdate : DtoBase
    {
        [JsonIgnore]
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public string? Courier { get; set; }
    }
}
