using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    internal class ChannelUsersConfiguration : IEntityTypeConfiguration<ChannelUsers>
    {
        public void Configure(EntityTypeBuilder<ChannelUsers> builder)
        {
            builder.HasKey(e => e.ChannelUsersId).HasName("t_channel_users_pkey");

            builder.ToTable("T_CHANNEL_USERS");

            builder.Property(e => e.ChannelUsersId).HasColumnName("ChannelUsersID");

            builder.HasOne(d => d.IdAthleteNavigation).WithMany(p => p.ChannelUsers)
                .HasForeignKey(d => d.IdAthlete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_channel_users_athleteid_fkey");

            builder.HasOne(d => d.IdChannelNavigation).WithMany(p => p.ChannelUsers)
                .HasForeignKey(d => d.IdChannel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_channel_users_channelid_fkey");
        }
    }
}
