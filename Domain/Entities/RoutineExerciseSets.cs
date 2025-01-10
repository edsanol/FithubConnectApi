namespace Domain.Entities
{
    public class RoutineExerciseSets
    {
        public long RoutineExerciseSetId { get; set; }

        public long IdRoutineExercise { get; set; }

        public int SetNumber { get; set; }

        public int? Reps { get; set; }

        public decimal? Weight { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual RoutineExercises IdRoutineExerciseNavigation { get; set; } = null!;
    }
}
