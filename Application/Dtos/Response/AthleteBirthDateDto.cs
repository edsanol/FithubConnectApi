namespace Application.Dtos.Response
{
    public class AthleteBirthDateDto
    {
        public int AthleteId { get; set; }
        public string AthleteName { get; set; } = string.Empty;
        public string AthleteLastName { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public int Age { get; set; }
    }
}
