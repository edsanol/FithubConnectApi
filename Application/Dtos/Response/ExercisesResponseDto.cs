namespace Application.Dtos.Response
{
    public class ExercisesResponseDto
    {
        public long ExerciseId { get; set; }
        public string ExerciseTitle { get; set; } = null!;
        public string ExerciseDescription { get; set; } = null!;
        public decimal Duration { get; set; }
        public string VideoURL { get; set; } = null!;
        public string ImageURL { get; set; } = null!;
        public long? IdMuscleGroup { get; set; }
        public string? MuscleGroupName { get; set; }
    }
}
