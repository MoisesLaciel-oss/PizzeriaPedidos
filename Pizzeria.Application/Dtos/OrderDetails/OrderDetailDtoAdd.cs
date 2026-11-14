using System.Text.Json.Serialization;

namespace Pizzeria.Application.Dtos.OrderDetails
{
    public class OrderDetailDtoAdd : DtoBase
    {
        [JsonIgnore]
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Quantity { get; set; }
    }
}
