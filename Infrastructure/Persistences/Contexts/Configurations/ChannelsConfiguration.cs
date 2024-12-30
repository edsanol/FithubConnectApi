using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ChannelsConfiguration : IEntityTypeConfiguration<Channels>
    {
        public void Configure(EntityTypeBuilder<Channels> builder)
        {
            builder.HasKey(e => e.ChannelId).HasName("t_channels_pkey");

            builder.ToTable("T_CHANNELS");

            builder.Property(e => e.ChannelId).HasColumnName("ChannelID");

            builder.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.HasOne(d => d.IdGymNavigation).WithMany(p => p.Channels)
                .HasForeignKey(d => d.IdGym)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_channels_gymid_fkey");
        }
    }
}
