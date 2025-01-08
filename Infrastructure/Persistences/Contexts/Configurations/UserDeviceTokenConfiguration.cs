using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class UserDeviceTokenConfiguration : IEntityTypeConfiguration<UserDeviceToken>
    {
        public void Configure(EntityTypeBuilder<UserDeviceToken> builder)
        {
            builder.HasKey(e => e.UserDeviceTokenId).HasName("t_user_device_token_pkey");

            builder.ToTable("T_USER_DEVICE_TOKEN");

            builder.Property(e => e.UserDeviceTokenId).HasColumnName("UserDeviceTokenID");

            builder.Property(e => e.CreadetAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.Property(e => e.LastUsedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.HasOne(d => d.IdAthleteNavigation).WithMany(p => p.UserDeviceTokens)
                .HasForeignKey(d => d.IdAthlete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_user_device_token_athleteid_fkey");
        }
    }
}
