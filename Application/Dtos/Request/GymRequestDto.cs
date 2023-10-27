namespace Application.Dtos.Request
{
    public class GymRequestDto
    {
        public string GymName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime RegisterDate { get; set; }
        public string SubscriptionPlan { get; set; } = string.Empty;
        public int MemberNumber { get; set; }
        public string? Comments { get; set; }
        public bool? Status { get; set; } = true;
        public string Nit { get; set; } = string.Empty;
    }
}
