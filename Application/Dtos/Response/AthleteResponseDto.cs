namespace Application.Dtos.Response
{
    public class AthleteResponseDto
    {
        public int AthleteId { get; set; }
        public string AthleteName { get; set; } = string.Empty;
        public string AthleteLastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string Genre { get; set; } = string.Empty;
        public bool? Status { get; set; }
        public string? StateAthlete { get; set; }
        public string? Token { get; set; } = null;
        public string? RefreshToken { get; set; } = null;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? MembershipName { get; set; }
        public decimal? Cost { get; set; }
        public int? MembershipId { get; set; }
        public string? CardAccessCode { get; set; } = string.Empty;
        public int IdGym { get; set; }
        public string? FingerPrint { get; set; }
    }
}
