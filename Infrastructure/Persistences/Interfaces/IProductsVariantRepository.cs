using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IProductsVariantRepository
    {
        Task<bool> RegisterProductVariant(ProductsVariant request);
        Task<ProductsVariant> GetProductVariantByProductId(int productId);
        Task<bool> EditProductVariant(ProductsVariant request);
    }
}
