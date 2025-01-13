namespace Domain.Entities
{
    public class HistoricalSets
    {
        public int HistoricalSetID { get; set; }

        public int IdRoutineExercise { get; set; }

        public int SetNumber { get; set; }

        public int Reps { get; set; }

        public decimal Weight { get; set; }

        public DateTime PerformedAt { get; set; }

        public int IdAthlete { get; set; }

        public virtual RoutineExercises IdRoutineExerciseNavigation { get; set; } = null!;

        public virtual Athlete IdAthleteNavigation { get; set; } = null!;
    }
}
