using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class NotificationsConfiguration : IEntityTypeConfiguration<Notifications>
    {
        public void Configure(EntityTypeBuilder<Notifications> builder)
        {
            builder.HasKey(e => e.NotificationId).HasName("t_notifications_pkey");

            builder.ToTable("T_NOTIFICATIONS");

            builder.Property(e => e.NotificationId).HasColumnName("NotificationID");

            builder.Property(e => e.SendAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            builder.HasOne(d => d.IdChannelNavigation).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.IdChannel)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("t_notification_channelid_fkey");
        }
    }
}
