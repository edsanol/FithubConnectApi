using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> DeleteExercisesFromRoutine(long routineId, List<long> DeleteExercises)
        {
            var routineExercises = await _context.RoutineExercises
                .Where(x => x.IdRoutine == routineId && DeleteExercises.Contains(x.IdExercise))
                .ToListAsync();

            routineExercises.ForEach(x => x.IsActive = false);

            _context.RoutineExercises.UpdateRange(routineExercises);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<RoutineExercises?> GetRoutineExerciseById(long routineExerciseId)
        {
            return await _context.RoutineExercises
                .FirstOrDefaultAsync(x => x.RoutineExerciseId == routineExerciseId);
        }

        public async Task<RoutineExercises?> GetRoutineExerciseByRoutineAndExercise(long routineId, long exerciseId)
        {
            return await _context.RoutineExercises
                .FirstOrDefaultAsync(x => x.IdRoutine == routineId && x.IdExercise == exerciseId);
        }

        public async Task<bool> IsExerciseInRoutine(long routineId, long exerciseId)
        {
            return await _context.RoutineExercises
                .AnyAsync(x => x.IdRoutine == routineId && x.IdExercise == exerciseId);
        }
    }
}
