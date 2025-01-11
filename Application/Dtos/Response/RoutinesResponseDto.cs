namespace Application.Dtos.Response
{
    public class RoutinesResponseDto
    {
        public long RoutineId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public long IdMuscleGroup { get; set; }
        public string MuscleGroupName { get; set; } = null!;
        public string ImageURL { get; set; } = null!;
        public bool IsActive { get; set; }
        public List<RoutineExerciseResponseDto> Exercises { get; set; } = new List<RoutineExerciseResponseDto>();
        public List<AthleteRoutinesResponseDto>? AthleteRoutines { get; set; } = new List<AthleteRoutinesResponseDto>();
    }
}
