namespace Domain.Entities
{
    public class AthleteRoutines
    {
        public int AthleteRoutineId { get; set; }

        public int IdAthlete { get; set; }

        public long IdRoutine { get; set; }

        public string Status { get; set; } = "Active";

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual Athlete IdAthleteNavigation { get; set; } = null!;

        public virtual Routines IdRoutineNavigation { get; set; } = null!;
    }
}
