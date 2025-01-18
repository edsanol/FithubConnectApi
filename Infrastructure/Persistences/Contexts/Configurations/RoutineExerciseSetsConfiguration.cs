using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class RoutineExerciseSetsConfiguration : IEntityTypeConfiguration<RoutineExerciseSets>
    {
        public void Configure(EntityTypeBuilder<RoutineExerciseSets> builder)
        {
            builder.HasKey(e => e.RoutineExerciseSetId).HasName("t_routine_exercise_sets_pkey");

            builder.ToTable("T_ROUTINE_EXERCISE_SETS");

            builder.Property(e => e.RoutineExerciseSetId).HasColumnName("RoutineExerciseSetID");

            builder.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.HasOne(d => d.IdRoutineExerciseNavigation).WithMany(p => p.RoutineExerciseSets)
                .HasForeignKey(d => d.IdRoutineExercise)
                .OnDelete(DeleteBehavior.ClientNoAction)
                .HasConstraintName("t_routine_exercise_sets_routineexerciseid_fkey");
        }
    }
}
