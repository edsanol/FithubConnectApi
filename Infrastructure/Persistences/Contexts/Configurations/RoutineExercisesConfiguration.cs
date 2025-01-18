using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class RoutineExercisesConfiguration : IEntityTypeConfiguration<RoutineExercises>
    {
        public void Configure(EntityTypeBuilder<RoutineExercises> builder)
        {
            builder.HasKey(e => e.RoutineExerciseId).HasName("t_routine_exercises_pkey");

            builder.ToTable("T_ROUTINE_EXERCISES");

            builder.Property(e => e.RoutineExerciseId).HasColumnName("RoutineExerciseID");

            builder.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.HasOne(d => d.IdExerciseNavigation)
                .WithMany(p => p.RoutineExercises)
                .HasForeignKey(d => d.IdExercise)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_routine_exercises_exerciseid_fkey");

            builder.HasOne(d => d.IdRoutineNavigation)
                .WithMany(p => p.RoutineExercises)
                .HasForeignKey(d => d.IdRoutine)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_routine_exercises_routineid_fkey");
        }
    }
}
