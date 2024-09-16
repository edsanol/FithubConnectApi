namespace Domain.Entities;

public partial class CardAccess
{
    public int CardId { get; set; }

    public int IdAthlete { get; set; }

    public string CardNumber { get; set; } = null!;

    public DateOnly? ExpirationDate { get; set; }

    public bool? Status { get; set; }

    public virtual Athlete IdAthleteNavigation { get; set; } = null!;
}
