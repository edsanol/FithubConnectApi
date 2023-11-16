namespace Application.Dtos.Response
{
    public class DiscountSelectResponseDto
    {
        public int DiscountID { get; set; }
        public string MembershipName { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
    }
}
