using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class HistoricalSetsConfiguration : IEntityTypeConfiguration<HistoricalSets>
    {
        public void Configure(EntityTypeBuilder<HistoricalSets> builder)
        {
            builder.HasKey(e => e.HistoricalSetID).HasName("t_historicalsets_pkey");

            builder.ToTable("T_HISTORICAL_SETS");

            builder.Property(e => e.HistoricalSetID).HasColumnName("HistoricalSetID");

            builder.Property(e => e.PerformedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.HasOne(d => d.IdRoutineExerciseNavigation).WithMany(p => p.HistoricalSets)
                .HasForeignKey(d => d.IdRoutineExercise)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("t_historical_sets_routineexerciseid_fkey");

            builder.HasOne(d => d.IdAthleteNavigation).WithMany(p => p.HistoricalSets)
                .HasForeignKey(d => d.IdAthlete)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("t_historical_sets_athleteid_fkey");
        }
    }
}
