namespace Domain.Entities;

public partial class Membership
{
    public int MembershipId { get; set; }

    public string MembershipName { get; set; } = null!;

    public decimal Cost { get; set; }

    public int DurationInDays { get; set; }

    public string? Description { get; set; }

    public int IdGym { get; set; }

    public virtual Gym IdGymNavigation { get; set; } = null!;

    public virtual ICollection<AthleteMembership> AthleteMemberships { get; set; } = new List<AthleteMembership>();

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}
