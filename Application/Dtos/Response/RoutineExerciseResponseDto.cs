namespace Application.Dtos.Response
{
    public class RoutineExerciseResponseDto
    {
        public long RoutineExerciseId { get; set; }
        public long IdExercise { get; set; }
        public string ExerciseTitle { get; set; } = null!;
        public string ExerciseDescription { get; set; } = null!;
        public decimal Duration { get; set; }
        public string VideoURL { get; set; } = null!;
        public string ImageURL { get; set; } = null!;
        public List<RoutineExerciseSetsResponseDto> RoutineExerciseSets { get; set; } = new List<RoutineExerciseSetsResponseDto>();
    }
}
