using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class AthleteMembershipConfiguration : IEntityTypeConfiguration<AthleteMembership>
    {
        public void Configure(EntityTypeBuilder<AthleteMembership> builder)
        {
            builder.HasKey(e => e.AthleteMembershipId).HasName("primary_key athlete membership");

            builder.ToTable("T_ATHLETE_MEMBERSHIP");

            builder.Property(e => e.AthleteMembershipId)
                .HasColumnName("AthleteMembershipID");

            builder.HasOne(d => d.IdAthleteNavigation).WithMany(p => p.AthleteMemberships)
                .HasForeignKey(d => d.IdAthlete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("athlete_membership_relation");

            builder.HasOne(d => d.IdMembershipNavigation).WithMany(p => p.AthleteMemberships)
                .HasForeignKey(d => d.IdMembership)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("membership_relation");
        }
    }
}
