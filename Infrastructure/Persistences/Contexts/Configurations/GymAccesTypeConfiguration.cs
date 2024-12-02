using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GymAccesTypeConfiguration : IEntityTypeConfiguration<GymAccessType>
    {
        public void Configure(EntityTypeBuilder<GymAccessType> builder)
        {
            builder.HasKey(e => e.GymAccessTypeId).HasName("T_GYM_ACCESS_TYPE_pkey");

            builder.ToTable("T_GYM_ACCESS_TYPE");

            builder.Property(e => e.GymAccessTypeId).HasColumnName("GymAccessTypeID");

            builder.Property(e => e.IdAccessType).HasColumnName("IdAccessType");

            builder.Property(e => e.IdGym).HasColumnName("IdGym");

            builder.HasOne(d => d.IdAccessTypeNavigation).WithMany(p => p.GymAccessTypes)
                .HasForeignKey(d => d.IdAccessType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_gym_access_to_access_type");

            builder.HasOne(d => d.IdGymNavigation).WithMany(p => p.GymAccessTypes)
                .HasForeignKey(d => d.IdGym)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_gym_access_to_gym");
        }
    }
}
