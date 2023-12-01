using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class DiscountRepository : GenericRepository<Discount>, IDiscountRepository
    {
        private readonly DbFithubContext _context;

        public DiscountRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CreateDiscount(Discount discount)
        {
            await _context.AddAsync(discount);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }

        public async Task<bool> DeleteDiscount(int discountID)
        {
            var discount = await _context.Discounts.AsNoTracking().SingleOrDefaultAsync(x => x.DiscountId.Equals(discountID));

            if (discount is null)
                return false;

            discount.Status = false;
            _context.Update(discount);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }

        public async Task<bool> DiscountExists(int membershipID)
        {
            var discounts = (from c in _context.Discounts
                             select c).AsNoTracking().AsQueryable();

            var discount = await discounts.Where(x => x.IdMembership.Equals(membershipID)).ToListAsync();

            if (discount is null) return false;

            foreach (var item in discount)
            {
                if (Convert.ToDateTime(item.EndDate) >= DateTime.Now)
                    return true;
            }

            return false;
        }

        public async Task<Discount> GetDiscountById(int discountID)
        {
            var discount = await _context.Discounts.AsNoTracking().SingleOrDefaultAsync(x => x.DiscountId.Equals(discountID));

            return discount!;
        }

        public async Task<BaseEntityResponse<Discount>> ListDiscounts(BaseFiltersRequest filters)
        {
            var response = new BaseEntityResponse<Discount>();

            var discounts = (from c in _context.Discounts
                             select c).AsNoTracking().AsQueryable();

            if (filters.NumFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                switch (filters.NumFilter)
                {
                    case 1:
                        discounts = discounts.Where(x => x.IdGym.Equals(int.Parse(filters.TextFilter)));
                        break;
                    default:
                        break;

                }
            }

            if (filters.StateFilter is not null)
            {
                discounts = discounts.Where(x => x.Status.Equals(filters.StateFilter));
            }

            if (!string.IsNullOrEmpty(filters.StartDate))
            {
                discounts = discounts.Where(x => Convert.ToDateTime(x.StartDate) >= Convert.ToDateTime(filters.StartDate));
            }

            if (!string.IsNullOrEmpty(filters.EndDate))
            {
                discounts = discounts.Where(x => Convert.ToDateTime(x.EndDate) >= Convert.ToDateTime(filters.EndDate));
            }

            filters.Sort ??= "DiscountId";

            response.TotalRecords = await discounts.Where(x => x.IdGym.Equals(int.Parse(filters.TextFilter))).CountAsync();
            response.Items = await discounts.Where(x => x.IdGym.Equals(int.Parse(filters.TextFilter))).ToListAsync();

            return response;
        }

        public async Task<IEnumerable<Discount>> ListSelectDiscounts(int gymID)
        {
            var discounts = await _context.Discounts
                .Where(x => x.IdGym.Equals(gymID) && x.Status.Equals(true))
                .ToListAsync();

            return discounts;
        }

        public async Task<bool> UpdateDiscount(Discount discount)
        {
            _context.Update(discount);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
