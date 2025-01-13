namespace Domain.Entities
{
    public class RoutineExercises
    {
        public long RoutineExerciseId { get; set; }

        public long IdRoutine { get; set; }

        public long IdExercise { get; set; }

        public int Order { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual Exercises IdExerciseNavigation { get; set; } = null!;

        public virtual Routines IdRoutineNavigation { get; set; } = null!;

        public virtual ICollection<RoutineExerciseSets> RoutineExerciseSets { get; set; } = new List<RoutineExerciseSets>();

        public static implicit operator RoutineExercises(bool v)
        {
            throw new NotImplementedException();
        }
    }
}
