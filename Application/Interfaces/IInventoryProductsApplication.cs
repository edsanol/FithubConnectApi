using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IInventoryProductsApplication
    {
        Task<BaseResponse<bool>> RegisterCategoryProduct(CategoryProductsRequestDto request);
        Task<BaseResponse<IEnumerable<CategoryProductsResponseDto>>> GetAllCategoriesProducts();
        Task<BaseResponse<bool>> RegisterProduct(ProductsRequestDto request);
        Task<BaseResponse<BaseEntityResponse<ProductsResponseDto>>> GetAllProducts(BaseFiltersRequest filters);
        Task<BaseResponse<bool>> EditProduct(int productId, EditProductRequestDto request);
        Task<BaseResponse<bool>> DeleteProduct(int productId);
        Task<BaseResponse<bool>> RegisterEntryAndExitProduct(EntryAndExitProductRequestDto request);
    }
}
