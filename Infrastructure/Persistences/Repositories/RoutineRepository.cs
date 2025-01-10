using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class RoutineRepository : GenericRepository<Routines>, IRoutineRepository
    {
        private readonly DbFithubContext _context;

        public RoutineRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CreateRoutine(Routines routine)
        {
            await _context.Routines.AddAsync(routine);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
