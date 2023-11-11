using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class AthleteConfiguration : IEntityTypeConfiguration<Athlete>
    {
        public void Configure(EntityTypeBuilder<Athlete> builder)
        {
            builder.HasKey(e => e.AthleteId).HasName("primary_key athlete");

            builder.ToTable("T_ATHLETE");

            builder.HasIndex(e => e.Email, "unique email").IsUnique();

            builder.Property(e => e.AthleteId).HasColumnName("AthleteID");
            builder.Property(e => e.AuditCreateDate).HasColumnType("timestamp without time zone");
            builder.Property(e => e.AuditDeleteDate).HasColumnType("timestamp without time zone");
            builder.Property(e => e.AuditUpdateDate).HasColumnType("timestamp without time zone");
            builder.Property(e => e.Status).HasDefaultValueSql("true");

            builder.HasOne(d => d.IdGymNavigation).WithMany(p => p.Athletes)
                .HasForeignKey(d => d.IdGym)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reference gym to athlete");
        }
    }
}
