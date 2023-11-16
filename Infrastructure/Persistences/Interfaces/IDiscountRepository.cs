using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;

namespace Infrastructure.Persistences.Interfaces
{
    public interface IDiscountRepository
    {
        Task<BaseEntityResponse<Discount>> ListDiscounts(BaseFiltersRequest filters);
        Task<IEnumerable<Discount>> ListSelectDiscounts(int gymID);
        Task<Discount> GetDiscountById(int discountID);
        Task<bool> CreateDiscount(Discount discount);
        Task<bool> UpdateDiscount(Discount discount);
        Task<bool> DeleteDiscount(int discountID);
    }
}
