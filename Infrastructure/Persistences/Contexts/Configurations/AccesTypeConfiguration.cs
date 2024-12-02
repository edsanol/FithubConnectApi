using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class AccesTypeConfiguration : IEntityTypeConfiguration<AccessType>
    {
        public void Configure(EntityTypeBuilder<AccessType> builder)
        {
            builder.HasKey(e => e.AccessTypeId).HasName("T_ACCESS_TYPE_pkey");

            builder.ToTable("T_ACCESS_TYPE");

            builder.Property(e => e.AccessTypeId).HasColumnName("AccessTypeID");

            builder.Property(e => e.AccessTypeName).IsRequired();

            builder.Property(e => e.Status).IsRequired();
        }
    }
}
