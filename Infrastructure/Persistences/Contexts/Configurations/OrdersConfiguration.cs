using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class OrdersConfiguration : IEntityTypeConfiguration<Orders>
    {
        public void Configure(EntityTypeBuilder<Orders> builder)
        {
            builder.HasKey(e => e.OrderId).HasName("t_orders_pkey");

            builder.ToTable("T_ORDERS");

            builder.Property(e => e.OrderId).HasColumnName("OrderID");

            builder.Property(e => e.OrderDate).HasColumnType("timestamp without time zone");

            builder.Property(e => e.ShippingAddress).IsRequired();

            builder.Property(e => e.Status).IsRequired();

            builder.Property(e => e.TotalAmount).HasColumnType("numeric");

            builder.Property(e => e.TotalPaid).HasColumnType("numeric");

            builder.HasOne(d => d.IdAthleteNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdAthlete)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_orders_athleteid_fkey");

            builder.HasOne(d => d.IdGymNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdGym)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_orders_gymid_fkey");
        }
    }
}
