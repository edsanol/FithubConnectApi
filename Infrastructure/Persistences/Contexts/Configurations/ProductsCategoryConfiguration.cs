using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Contexts.Configurations
{
    public class ProductsCategoryConfiguration : IEntityTypeConfiguration<ProductsCategory>
    {
        public void Configure(EntityTypeBuilder<ProductsCategory> builder)
        {
            builder.HasKey(e => e.CategoryId).HasName("t_products_category_pkey");

            builder.ToTable("T_PRODUCTS_CATEGORY");

            builder.Property(e => e.CategoryId).HasColumnName("CategoryID");

            builder.HasOne(d => d.IdGymNavigation).WithMany(p => p.ProductsCategory)
                .HasForeignKey(d => d.IdGym)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("category_gym_fk");
        }
    }
}
