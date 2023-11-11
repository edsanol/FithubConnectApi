namespace Domain.Entities;

public partial class Discount
{
    public int DiscountId { get; set; }

    public int IdMembership { get; set; }

    public decimal DiscountPercentage { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual Membership IdMembershipNavigation { get; set; } = null!;
}
