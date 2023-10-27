using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ExerciseMetric
{
    public int ExerciseMetricId { get; set; }

    public int IdExerciseType { get; set; }

    public string MetricName { get; set; } = null!;

    public virtual ExerciseType IdExerciseTypeNavigation { get; set; } = null!;
}
