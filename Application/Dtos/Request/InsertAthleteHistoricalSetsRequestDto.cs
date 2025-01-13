namespace Application.Dtos.Request
{
    public class InsertAthleteHistoricalSetsRequestDto
    {
        public int IdRoutineExercise { get; set; }
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public decimal Weight { get; set; }
    }
}
