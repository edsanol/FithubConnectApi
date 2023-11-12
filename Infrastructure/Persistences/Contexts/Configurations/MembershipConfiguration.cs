using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    internal class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.HasKey(e => e.MembershipId).HasName("primary_key membership");

            builder.ToTable("T_MEMBERSHIP");

            builder.Property(e => e.MembershipId)
                .HasColumnName("MembershipID");

            builder.HasOne(d => d.IdGymNavigation).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.IdGym)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("gym_membership_relation");
        }
    }
}
