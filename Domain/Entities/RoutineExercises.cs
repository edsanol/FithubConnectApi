namespace Domain.Entities
{
    public class RoutineExercises
    {
        public long RoutineExerciseId { get; set; }

        public long IdRoutine { get; set; }

        public long IdExercise { get; set; }

        public int Order { get; set; }

        public int Sets { get; set; }

        public int Reps { get; set; }

        public float Weight { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual Exercises IdExerciseNavigation { get; set; } = null!;

        public virtual Routines IdRoutineNavigation { get; set; } = null!;
    }
}
