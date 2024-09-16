using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class AccessLogConfiguration : IEntityTypeConfiguration<AccessLog>
    {
        public void Configure(EntityTypeBuilder<AccessLog> builder)
        {
            builder.HasKey(e => e.AccessId).HasName("primary_key access log");

            builder.ToTable("T_ACCESS_LOG");

            builder.Property(e => e.AccessId).HasColumnName("AccessID");

            builder.HasOne(d => d.IdAthleteNavigation).WithMany(p => p.AccessLogs)
                .HasForeignKey(d => d.IdAthlete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("athlete and access log relation");

            builder.HasOne(d => d.IdGymNavigation).WithMany(p => p.AccessLogs)
                .HasForeignKey(d => d.IdGym)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("gym and access log relation");
        }
    }
}
