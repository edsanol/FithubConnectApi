namespace Application.Dtos.Request
{
    public class DiscountRequestDto
    {
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int IdMembership { get; set; }
        public int IdGym { get; set; }
        public string? Comments { get; set; }
    }
}
