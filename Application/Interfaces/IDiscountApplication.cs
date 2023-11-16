using Application.Commons.Bases;
using Application.Dtos.Request;
using Application.Dtos.Response;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IDiscountApplication
    {
        Task<BaseResponse<BaseEntityResponse<DiscountResponseDto>>> ListDiscounts(BaseFiltersRequest filters);
        Task<BaseResponse<IEnumerable<DiscountSelectResponseDto>>> ListDiscountsSelect(int gymID);
        Task<BaseResponse<DiscountResponseDto>> DiscountById(int discountID);
        Task<BaseResponse<bool>> CreateDiscount(DiscountRequestDto discountID);
        Task<BaseResponse<bool>> UpdateDiscount(int discountID, DiscountRequestDto discount);
        Task<BaseResponse<bool>> DeleteDiscount(int discountID);
    }
}
