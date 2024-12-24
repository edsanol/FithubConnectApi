namespace Application.Dtos.Response
{
    public class GymResponseDto
    {
        public int GymId { get; set; }
        public string EncryptedId { get; set; } = string.Empty;
        public string GymName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string SubscriptionPlan { get; set; } = string.Empty;
        public int MemberNumber { get; set; }
        public string? Comments { get; set; }
        public bool? Status { get; set; }
        public string? StateGym { get; set; }
        public string? Token { get; set; } = null;
        public string? RefreshToken { get; set; } = null;
        public string? Nit { get; set; } = null;
        public List<AccessTypeResponseDto> AccessTypes { get; set; } = new List<AccessTypeResponseDto>();
    }
}
