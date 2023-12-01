namespace Application.Dtos.Request
{
    public class MembershipRequestDto
    {
        public string MembershipName { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public int DurationInDays { get; set; }
        public string? Description { get; set; }
        public int IdGym { get; set; }
        public bool Status { get; set; }
    }
}
