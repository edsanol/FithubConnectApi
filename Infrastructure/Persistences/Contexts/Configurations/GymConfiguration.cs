using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class GymConfiguration : IEntityTypeConfiguration<Gym>
    {
        public void Configure(EntityTypeBuilder<Gym> builder)
        {
            builder.HasKey(e => e.GymId).HasName("primary_key");

            builder.ToTable("T_GYM");

            builder.Property(e => e.GymId).HasColumnName("GymID");
            builder.Property(e => e.Status)
                .IsRequired()
                .HasDefaultValueSql("true");

            builder.HasIndex(e => e.Email, "email_unique").IsUnique();
        }
    }
}
