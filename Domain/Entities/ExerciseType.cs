using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ExerciseType
{
    public int ExerciseTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<AthleteProgress> AthleteProgresses { get; set; } = new List<AthleteProgress>();

    public virtual ICollection<ExerciseMetric> ExerciseMetrics { get; set; } = new List<ExerciseMetric>();
}
