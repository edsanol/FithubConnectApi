namespace Domain.Entities
{
    public class Routines
    {
        public long RoutineId { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public long IdMuscleGroup { get; set; }

        public string ImageURL { get; set; } = null!;

        public int IdGym { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; }

        public virtual MuscleGroups IdMuscleGroupNavigation { get; set; } = null!;

        public virtual Gym IdGymNavigation { get; set; } = null!;

        public virtual ICollection<RoutineExercises> RoutineExercises { get; set; } = new List<RoutineExercises>();

        public virtual ICollection<AthleteRoutines> AthleteRoutines { get; set; } = new List<AthleteRoutines>();
    }
}
