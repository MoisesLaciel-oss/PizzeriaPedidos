using System.Text.Json.Serialization;

namespace Pizzeria.Application.Dtos.Customers
{
    public class CustomerDtoUpdate : CustomerDtoAdd
    {
        [JsonIgnore]
        public int Id { get; set; }
    }
}
