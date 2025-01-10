using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class RoutineExerciseRepository : GenericRepository<RoutineExercises>, IRoutineExerciseRepository
    {
        private readonly DbFithubContext _context;

        public RoutineExerciseRepository(DbFithubContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateRoutineExercise(RoutineExercises routineExercise)
        {
            await _context.RoutineExercises.AddAsync(routineExercise);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
