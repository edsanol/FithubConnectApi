using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GymTokenConfiguration : IEntityTypeConfiguration<GymToken>
    {
        public void Configure(EntityTypeBuilder<GymToken> builder)
        {
            builder.HasKey(e => e.TokenID).HasName("primary_key gym token");

            builder.ToTable("T_GYM_TOKEN");

            builder.Property(e => e.TokenID).HasColumnName("TokenID");

            builder.HasOne(d => d.IdGymNavigation).WithMany(p => p.GymToken)
                .HasForeignKey(d => d.IdGym)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("gym and gym token relation");
        }
    }
}
