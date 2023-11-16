namespace Domain.Entities;

public partial class Discount
{
    public int DiscountId { get; set; }

    public int IdMembership { get; set; }

    public decimal DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool Status { get; set; }

    public int IdGym { get; set; }

    public string? Comments { get; set; }

    public virtual Membership IdMembershipNavigation { get; set; } = null!;

    public virtual Gym IdGymNavigation { get; set; } = null!;
}
