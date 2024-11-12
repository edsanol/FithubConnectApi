using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    internal class StockMovementsConfiguration : IEntityTypeConfiguration<StockMovements>
    {
        public void Configure(EntityTypeBuilder<StockMovements> builder)
        {
            builder.HasKey(e => e.MovementId).HasName("t_stockmovements_pkey");

            builder.ToTable("T_STOCK_MOVEMENTS");

            builder.Property(e => e.MovementId).HasColumnName("MovementID");

            builder.Property(e => e.MovementDate).HasColumnType("timestamp with time zone");

            builder.Property(e => e.MovementType).IsRequired();

            builder.Property(e => e.Notes).HasDefaultValueSql("''");

            builder.Property(e => e.Quantity).HasDefaultValueSql("1");

            builder.HasOne(d => d.IdRelatedOrderNavigation).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.IdRelatedOrder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_stockmovements_orderid_fkey");

            builder.HasOne(d => d.IdVariantNavigation).WithMany(p => p.StockMovements)
                .HasForeignKey(d => d.IdVariant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_stockmovements_variantid_fkey");
        }
    }
}
