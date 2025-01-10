using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class AthleteRoutinesConfiguration : IEntityTypeConfiguration<AthleteRoutines>
    {
        public void Configure(EntityTypeBuilder<AthleteRoutines> builder)
        {
            builder.HasKey(e => e.AthleteRoutineId).HasName("t_athleteroutines_pkey");

            builder.ToTable("T_ATHLETE_ROUTINES");

            builder.Property(e => e.AthleteRoutineId).HasColumnName("AthleteRoutineID");

            builder.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.HasOne(d => d.IdAthleteNavigation)
                .WithMany(p => p.AthleteRoutines)
                .HasForeignKey(d => d.IdAthlete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_athlete_routines_athleteid_fkey");

            builder.HasOne(d => d.IdRoutineNavigation)
                .WithMany(p => p.AthleteRoutines)
                .HasForeignKey(d => d.IdRoutine)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_athlete_routines_routineid_fkey");
        }
    }
}
