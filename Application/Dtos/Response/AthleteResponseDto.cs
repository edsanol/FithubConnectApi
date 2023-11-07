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
    }
}
