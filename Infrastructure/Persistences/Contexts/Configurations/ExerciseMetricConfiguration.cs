using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ExerciseMetricConfiguration : IEntityTypeConfiguration<ExerciseMetric>
    {
        public void Configure(EntityTypeBuilder<ExerciseMetric> builder)
        {
            builder.HasKey(e => e.ExerciseMetricId).HasName("primary_key exercise metric");

            builder.ToTable("T_EXERCISE_METRIC");

            builder.Property(e => e.ExerciseMetricId).HasColumnName("ExerciseMetricID");

            builder.HasOne(d => d.IdExerciseTypeNavigation).WithMany(p => p.ExerciseMetrics)
                .HasForeignKey(d => d.IdExerciseType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("exercise metric and exercise type relation");
        }
    }
}
