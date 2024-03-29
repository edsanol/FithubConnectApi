namespace Application.Dtos.Request
{
    public class AthleteEditRequestDto
    {
        public string AthleteName { get; set; } = string.Empty;
        public string AthleteLastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string Genre { get; set; } = string.Empty;
    }
}
