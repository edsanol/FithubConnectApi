using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class MuscleGroupRepository : GenericRepository<MuscleGroups>, IMuscleGroupRepository
    {
        private readonly DbFithubContext _context;

        public MuscleGroupRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<MuscleGroups>> GetMuscleGroups()
        {
            return await _context.MuscleGroups.ToListAsync();
        }
    }
}
