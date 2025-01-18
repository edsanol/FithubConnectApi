namespace Application.Dtos.Request
{
    public class UpdateRoutineRequestDto
    {
        public long RoutineId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public long? IdMuscleGroup { get; set; }
        public string? ImageURL { get; set; }
        public List<UpdateExerciseDto>? Exercises { get; set; }
        public List<long>? DeleteExercises { get; set; }
        public List<long>? DeleteSets { get; set; }
    }
}
