using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class AthleteMembershipRepository : GenericRepository<AthleteMembership>, IAthleteMembershipRepository
    {
        private readonly DbFithubContext _context;

        public AthleteMembershipRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> RegisterAthleteMembership(AthleteMembership athleteMembership)
        {
            await _context.AthleteMemberships.AddAsync(athleteMembership);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
