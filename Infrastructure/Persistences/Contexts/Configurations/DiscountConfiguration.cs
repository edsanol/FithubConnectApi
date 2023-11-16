using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.HasKey(e => e.DiscountId).HasName("primary_key discount");

            builder.ToTable("T_DISCOUNTS");

            builder.Property(e => e.DiscountId)
                .HasColumnName("DiscountID");

            builder.HasOne(d => d.IdMembershipNavigation).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.IdMembership)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("membership_discount_relation");

            builder.Property(e => e.Status).HasDefaultValueSql("true");

            builder.HasOne(d => d.IdGymNavigation).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.IdGym)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("gym_discount_relation");
        }
    }
}
