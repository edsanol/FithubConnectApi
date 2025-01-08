using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class MuscleGroupsConfiguration : IEntityTypeConfiguration<MuscleGroups>
    {
        public void Configure(EntityTypeBuilder<MuscleGroups> builder)
        {
            builder.HasKey(e => e.MuscleGroupId).HasName("t_muscle_groups_pkey");

            builder.ToTable("T_MUSCLE_GROUPS");

            builder.Property(e => e.MuscleGroupId).HasColumnName("MuscleGroupID");

            builder.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");
        }
    }
}
