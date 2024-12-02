namespace Application.Dtos.Request
{
    public class OrderRequestDto
    {
        public int AthleteId { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public List<OrderDetailInOrderRequestDto> OrderDetails { get; set; } = new List<OrderDetailInOrderRequestDto>();
    }
}
