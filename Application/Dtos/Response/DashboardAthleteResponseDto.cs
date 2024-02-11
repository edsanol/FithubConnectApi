namespace Application.Dtos.Response
{
    public class DashboardAthleteResponseDto
    {
        public int TotalAthletes { get; set; }
        public int ActiveAthletes { get; set; }
        public float ActiveAthletesPercentage { get; set; }
        public int InactiveAthletes { get; set; }
        public float InactiveAthletesPercentage { get; set; }
        public int DailyAssistance { get; set; }
        public int NewAthletesByMonth { get; set; }
        public decimal IncomeByMonth { get; set; }
    }
}
