using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class AccessLog
{
    public int AccessId { get; set; }

    public int IdAthlete { get; set; }

    public int? IdCard { get; set; }

    public DateTime AccessDateTime { get; set; }

    public int IdGym { get; set; }

    public int AccessType { get; set; }

    public virtual Athlete IdAthleteNavigation { get; set; } = null!;

    public virtual Gym IdGymNavigation { get; set; } = null!;
}
