using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    internal class GymAccessTypeRepository : GenericRepository<GymAccessType>, IGymAccessTypeRepository
    {
        private readonly DbFithubContext _context;

        public GymAccessTypeRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CreateGymAccessType(GymAccessType gymAccessType)
        {
            await _context.GymAccessTypes.AddAsync(gymAccessType);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteGymAccessType(int accessTypeID)
        {
            var gymAccessType = await _context.GymAccessTypes.FindAsync(accessTypeID);

            if (gymAccessType == null)
                return false;

            _context.GymAccessTypes.Remove(gymAccessType);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<GymAccessType>> GetGymAccessTypesByGymID(int gymID)
        {
            return await _context.GymAccessTypes.Where(x => x.IdGym == gymID).ToListAsync();
        }
    }
}
