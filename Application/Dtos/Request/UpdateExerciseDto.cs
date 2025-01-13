namespace Application.Dtos.Request
{
    public class UpdateExerciseDto
    {
        public long? RoutineExerciseId { get; set; }
        public long? IdExercise { get; set; }
        public NewExerciseRequestDto? NewExercise { get; set; }
        public List<UpdateSetDto> Sets { get; set; } = new List<UpdateSetDto>();
    }
}
