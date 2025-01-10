namespace Application.Dtos.Request
{
    public class ExerciseRequestDto
    {
        public long? IdExercise { get; set; }
        public NewExerciseRequestDto? NewExercise { get; set; }
        public List<SetRequestDto> Sets { get; set; } = new List<SetRequestDto>();
    }
}
