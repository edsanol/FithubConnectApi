using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class RoutinesConfiguration : IEntityTypeConfiguration<Routines>
    {
        public void Configure(EntityTypeBuilder<Routines> builder)
        {
            builder.HasKey(e => e.RoutineId).HasName("t_routines_pkey");

            builder.ToTable("T_ROUTINES");

            builder.Property(e => e.RoutineId).HasColumnName("RoutineID");

            builder.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.HasOne(d => d.IdMuscleGroupNavigation)
                .WithMany(p => p.Routines)
                .HasForeignKey(d => d.IdMuscleGroup)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_routines_musclegroupid_fkey");

            builder.HasOne(d => d.IdGymNavigation)
                .WithMany(p => p.Routines)
                .HasForeignKey(d => d.IdGym)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_routines_gymid_fkey");
        }
    }
}
