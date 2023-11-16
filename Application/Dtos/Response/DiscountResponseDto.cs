namespace Application.Dtos.Response
{
    public class DiscountResponseDto
    {
        public int DiscountId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }
        public int IdMembership { get; set; }
        public string? Comments { get; set; }
    }
}
