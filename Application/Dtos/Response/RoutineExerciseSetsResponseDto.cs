namespace Application.Dtos.Response
{
    public class RoutineExerciseSetsResponseDto
    {
        public long RoutineExerciseSetId { get; set; }
        public int SetNumber { get; set; }
        public int? Reps { get; set; }
        public decimal? Weight { get; set; }
    }
}
