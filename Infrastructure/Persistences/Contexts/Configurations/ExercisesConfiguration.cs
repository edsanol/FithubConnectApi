using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ExercisesConfiguration : IEntityTypeConfiguration<Exercises>
    {
        public void Configure(EntityTypeBuilder<Exercises> builder)
        {
            builder.HasKey(e => e.ExerciseId).HasName("t_exercise_pkey");

            builder.ToTable("T_EXERCISE");

            builder.Property(e => e.ExerciseId).HasColumnName("ExerciseID");

            builder.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.HasOne(d => d.IdGymNavigation)
                .WithMany(p => p.Exercises)
                .HasForeignKey(d => d.IdGym)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_exercise_gymid_fkey");

            builder.HasOne(d => d.IdMuscleGroupNavigation)
                .WithMany(p => p.Exercises)
                .HasForeignKey(d => d.IdMuscleGroup)
                .HasConstraintName("t_exercise_muscleid_fkey");
        }
    }
}
