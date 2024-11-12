using Domain.Entities;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IProductsCategoryRepository
    {
        Task<bool> RegisterCategoryProduct(ProductsCategory request);
        Task<IEnumerable<ProductsCategory>> GetAllCategoriesProducts(int gymID);
    }
}
