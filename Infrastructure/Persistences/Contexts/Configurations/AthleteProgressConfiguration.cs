using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class AthleteProgressConfiguration : IEntityTypeConfiguration<AthleteProgress>
    {
        public void Configure(EntityTypeBuilder<AthleteProgress> builder)
        {
            builder.HasKey(e => e.ProgressId).HasName("primary_key athlete progress");

            builder.ToTable("T_ATHLETE_PROGRESS");

            builder.Property(e => e.ProgressId).HasColumnName("ProgressID");

            builder.HasOne(d => d.IdAthleteNavigation).WithMany(p => p.AthleteProgresses)
                .HasForeignKey(d => d.IdAthlete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("athlete and athlete progress relation");

            builder.HasOne(d => d.IdExerciseTypeNavigation).WithMany(p => p.AthleteProgresses)
                .HasForeignKey(d => d.IdExerciseType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("exercise type and athlete progress relation");
        }
    }
}
