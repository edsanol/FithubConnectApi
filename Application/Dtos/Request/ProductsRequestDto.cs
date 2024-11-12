namespace Application.Dtos.Request
{
    public class ProductsRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int IdCategory { get; set; }
        public decimal BasePrice { get; set; }
        public int IdGym { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
