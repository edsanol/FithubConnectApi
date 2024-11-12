using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ProductsVariantConfiguration : IEntityTypeConfiguration<ProductsVariant>
    {
        public void Configure(EntityTypeBuilder<ProductsVariant> builder)
        {
            builder.HasKey(e => e.VariantId).HasName("t_products_variant_pkey");

            builder.ToTable("T_PRODUCTS_VARIANT");

            builder.Property(e => e.VariantId).HasColumnName("VariantID");

            builder.Property(e => e.Price).HasColumnType("numeric");

            builder.Property(e => e.SKU).IsRequired();

            builder.Property(e => e.StockQuantity).HasDefaultValueSql("0");

            builder.Property(e => e.IsActive).HasDefaultValueSql("true");

            builder.HasOne(d => d.Product).WithMany(p => p.ProductsVariants)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_products_variant_idproduct_fkey");
        }
    }
}
