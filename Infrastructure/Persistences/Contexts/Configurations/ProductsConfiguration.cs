using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ProductsConfiguration : IEntityTypeConfiguration<Products>
    {
        public void Configure(EntityTypeBuilder<Products> builder)
        {
            builder.HasKey(e => e.ProductId).HasName("t_products_pkey");

            builder.ToTable("T_PRODUCTS");

            builder.Property(e => e.ProductId).HasColumnName("ProductID");

            builder.Property(e => e.BasePrice).HasColumnType("numeric");

            builder.Property(e => e.Description).IsRequired();

            builder.Property(e => e.IsActive).HasDefaultValueSql("true");

            builder.Property(e => e.Name).IsRequired();

            builder.HasOne(d => d.IdGymNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdGym)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_products_gymid_fkey");

            builder.HasOne(d => d.IdProductsCategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("t_products_categoryid_fkey");
        }
    }
}
