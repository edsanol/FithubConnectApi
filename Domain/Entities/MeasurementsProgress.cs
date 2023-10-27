using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class MeasurementsProgress
{
    public int MeasurementsProgressId { get; set; }

    public int IdAthlete { get; set; }

    public decimal? Gluteus { get; set; }

    public decimal? Biceps { get; set; }

    public decimal? Chest { get; set; }

    public decimal? Waist { get; set; }

    public decimal? Hips { get; set; }

    public decimal? Thigh { get; set; }

    public decimal? Calf { get; set; }

    public decimal? Shoulders { get; set; }

    public decimal? Forearm { get; set; }

    public decimal? Height { get; set; }

    public decimal? Weight { get; set; }

    public DateOnly? Date { get; set; }

    public virtual Athlete IdAthleteNavigation { get; set; } = null!;
}
