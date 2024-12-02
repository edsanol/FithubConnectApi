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

    public virtual ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();

    public virtual ICollection<GymToken> GymToken { get; set; } = new List<GymToken>();

    public virtual ICollection<ProductsCategory> ProductsCategory { get; set; } = new List<ProductsCategory>();

    public virtual ICollection<Products> Products { get; set; } = new List<Products>();

    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();

    public virtual ICollection<GymAccessType> GymAccessTypes { get; set; } = new List<GymAccessType>();
}
