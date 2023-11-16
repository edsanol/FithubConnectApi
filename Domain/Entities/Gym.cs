namespace Domain.Entities;

public partial class Gym
{
    public int GymId { get; set; }

    public string GymName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public string Address { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public DateTime RegisterDate { get; set; }

    public string SubscriptionPlan { get; set; } = null!;

    public int MemberNumber { get; set; }

    public string? Comments { get; set; }

    public bool? Status { get; set; }

    public string Nit { get; set; } = null!;

    public virtual ICollection<Athlete> Athletes { get; set; } = new List<Athlete>();

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}
