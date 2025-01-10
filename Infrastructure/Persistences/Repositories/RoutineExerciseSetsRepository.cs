using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class RoutineExerciseSetsRepository : GenericRepository<RoutineExerciseSets>, IRoutineExerciseSetsRepository
    {
        private readonly DbFithubContext _context;

        public RoutineExerciseSetsRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CreateRoutineExerciseSets(RoutineExerciseSets routineExerciseSets)
        {
            await _context.RoutineExerciseSets.AddAsync(routineExerciseSets);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
