namespace Domain.Entities;

public partial class AthleteMembership
{
    public int AthleteMembershipId { get; set; }

    public int IdAthlete { get; set; }

    public int IdMembership { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual Athlete IdAthleteNavigation { get; set; } = null!;

    public virtual Membership IdMembershipNavigation { get; set; } = null!;
}
