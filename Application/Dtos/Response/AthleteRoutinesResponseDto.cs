namespace Application.Dtos.Response
{
    public class AthleteRoutinesResponseDto
    {
        public int IdAthlete { get; set; }
        public string NameAthlete { get; set; } = string.Empty;
        public string LastNameAthlete { get; set; } = string.Empty;
        public string EmailAthlete { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
