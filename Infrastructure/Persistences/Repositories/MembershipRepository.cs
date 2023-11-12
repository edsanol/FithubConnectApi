using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class MembershipRepository : GenericRepository<Membership>, IMembershipRepository
    {
        private readonly DbFithubContext _context;

        public MembershipRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CreateMembership(Membership membership)
        {
            await _context.AddAsync(membership);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }

        public async Task<bool> DeleteMembership(int membershipID)
        {
            var membership = await _context.Memberships.AsNoTracking().SingleOrDefaultAsync(x => x.MembershipId.Equals(membershipID));

            if (membership is null)
                return false;

            _context.Remove(membership);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }

        public async Task<Membership> GetMembershipById(int membershipID)
        {
            var membership = await _context.Memberships.AsNoTracking().SingleOrDefaultAsync(x => x.MembershipId.Equals(membershipID));

            return membership!;
        }

        public async Task<BaseEntityResponse<Membership>> ListMemberships(BaseFiltersRequest filters)
        {
            var response = new BaseEntityResponse<Membership>();

            var memberships = (from c in _context.Memberships
                               select c).AsNoTracking().AsQueryable();

            if (filters.NumFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                switch (filters.NumFilter)
                {
                    case 1:
                        memberships = memberships.Where(x => x.IdGym.Equals(int.Parse(filters.TextFilter)));
                        break;
                    default:
                        break;
                }
            }

            response.TotalRecords = await memberships.Where(x => x.IdGym.Equals(int.Parse(filters.TextFilter))).CountAsync();
            response.Items = await memberships.Where(x => x.IdGym.Equals(int.Parse(filters.TextFilter))).ToListAsync();

            return response;
        }

        public async Task<IEnumerable<Membership>> ListSelectMemberships(int gymID)
        {
            var memberships = await _context.Memberships
                .Where(x => x.IdGym.Equals(gymID)).AsNoTracking().ToListAsync();

            return memberships;
        }

        public async Task<bool> UpdateMembership(Membership membership)
        {
            _context.Update(membership);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
