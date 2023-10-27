namespace Domain.Entities;

public partial class AthleteProgress
{
    public int ProgressId { get; set; }

    public int IdAthlete { get; set; }

    public int IdExerciseType { get; set; }

    public decimal Value { get; set; }

    public DateOnly Date { get; set; }

    public virtual Athlete IdAthleteNavigation { get; set; } = null!;

    public virtual ExerciseType IdExerciseTypeNavigation { get; set; } = null!;
}
