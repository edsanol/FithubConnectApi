using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IProductsRepository
    {
        Task<bool> RegisterProduct(Products request);
        Task<BaseEntityResponse<Products>> GetAllProducts(BaseFiltersRequest filters, int gymID);
        Task<Products> GetProductById (int productId, int gymID);
        Task<bool> EditProduct(Products request);
    }
}
