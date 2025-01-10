namespace Application.Dtos.Request
{
    public class CreateRoutineRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long IdMuscleGroup { get; set; }
        public string ImageURL { get; set; } = string.Empty;
        public long IdChannel { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public List<ExerciseRequestDto> Exercises { get; set; } = new List<ExerciseRequestDto>();
    }
}
