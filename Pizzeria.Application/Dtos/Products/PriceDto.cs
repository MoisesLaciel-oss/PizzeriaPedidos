namespace Pizzeria.Application.Dtos.Products
{
    public class PriceDto : DtoBase
    {
        public int SizeId { get; set; }
        public decimal Price { get; set; }
    }
}
