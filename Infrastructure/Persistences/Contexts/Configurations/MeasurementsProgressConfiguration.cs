using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class MeasurementsProgressConfiguration : IEntityTypeConfiguration<MeasurementsProgress>
    {
        public void Configure(EntityTypeBuilder<MeasurementsProgress> builder)
        {
            builder.HasKey(e => e.MeasurementsProgressId).HasName("primary_key measurements progress");

            builder.ToTable("T_MEASUREMENTS_PROGRESS");

            builder.Property(e => e.MeasurementsProgressId).HasColumnName("MeasurementsProgressID");

            builder.HasOne(d => d.IdAthleteNavigation).WithMany(p => p.MeasurementsProgresses)
                .HasForeignKey(d => d.IdAthlete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("athlete and measurements progress relation");
        }
    }
}
