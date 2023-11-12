namespace Application.Dtos.Response
{
    public class MembershipResponseDto
    {
        public int MembershipID { get; set; }
        public string MembershipName { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public int DurationInDays { get; set; }
        public string? Description { get; set; }
    }
}
