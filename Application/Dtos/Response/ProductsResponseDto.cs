namespace Application.Dtos.Response
{
    public class ProductsResponseDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
