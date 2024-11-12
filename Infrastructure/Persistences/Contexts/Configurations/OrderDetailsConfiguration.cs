using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class OrderDetailsConfiguration : IEntityTypeConfiguration<OrderDetails>
    {
        public void Configure(EntityTypeBuilder<OrderDetails> builder)
        {
            builder.HasKey(e => e.OrderDetailId).HasName("t_orderdetails_pkey");

            builder.ToTable("T_ORDER_DETAILS");

            builder.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

            builder.Property(e => e.Quantity).HasDefaultValueSql("1");

            builder.Property(e => e.UnitPrice).HasColumnType("numeric");

            builder.Property(e => e.TotalPrice).HasColumnType("numeric");

            builder.Property(e => e.ReturnQuantity).HasDefaultValueSql("0");

            builder.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.IdOrder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_orderdetails_orderid_fkey");

            builder.HasOne(d => d.Variant).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.IdVariant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_orderdetails_variantid_fkey");
        }
    }
}
