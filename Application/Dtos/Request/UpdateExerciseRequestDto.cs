namespace Application.Dtos.Request
{
    public class UpdateExerciseRequestDto
    {
        public long ExerciseId { get; set; }
        public string ExerciseTitle { get; set; } = null!;
        public string ExerciseDescription { get; set; } = null!;
        public decimal Duration { get; set; }
        public string VideoURL { get; set; } = null!;
        public string ImageURL { get; set; } = null!;
        public long? IdMuscleGroup { get; set; }
        public bool IsActive { get; set; }
    }
}
