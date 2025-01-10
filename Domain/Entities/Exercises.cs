namespace Domain.Entities
{
    public class Exercises
    {
        public long ExerciseId { get; set; }

        public string ExerciseTitle { get; set; } = null!;

        public string ExerciseDescription { get; set; } = null!;

        public decimal Duration { get; set; }

        public string VideoURL { get; set; } = null!;

        public string ImageURL { get; set; } = null!;

        public int IdGym { get; set; }

        public long? IdMuscleGroup { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual Gym IdGymNavigation { get; set; } = null!;

        public virtual MuscleGroups IdMuscleGroupNavigation { get; set; } = null!;

        public virtual ICollection<RoutineExercises> RoutineExercises { get; set; } = new List<RoutineExercises>();
    }
}
