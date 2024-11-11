namespace Application.Dtos.Request
{
    public class EditProductRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal Price { get; set; }
    }
}
