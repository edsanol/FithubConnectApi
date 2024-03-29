namespace Domain.Entities;

public partial class Athlete
{
    public int AthleteId { get; set; }

    public string AthleteName { get; set; } = null!;

    public string AthleteLastName { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Genre { get; set; } = null!;

    public string? Password { get; set; }

    public int IdGym { get; set; }

    public string? AuditCreateUser { get; set; }

    public DateTime? AuditCreateDate { get; set; } = null;

    public string? AuditUpdateUser { get; set; }

    public DateTime? AuditUpdateDate { get; set; } = null;

    public string? AuditDeleteUser { get; set; }

    public DateTime? AuditDeleteDate { get; set; } = null;

    public bool? Status { get; set; }

    public virtual Gym IdGymNavigation { get; set; } = null!;

    public virtual ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();

    public virtual ICollection<AthleteMembership> AthleteMemberships { get; set; } = new List<AthleteMembership>();

    public virtual ICollection<AthleteProgress> AthleteProgresses { get; set; } = new List<AthleteProgress>();

    public virtual ICollection<CardAccess> CardAccesses { get; set; } = new List<CardAccess>();

    public virtual ICollection<MeasurementsProgress> MeasurementsProgresses { get; set; } = new List<MeasurementsProgress>();

    public virtual ICollection<AthleteToken> AthleteToken { get; set; } = new List<AthleteToken>();
}
