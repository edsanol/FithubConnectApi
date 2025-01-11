using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> DeleteSets(List<long> DeleteSets)
        {
            var routineExerciseSets = await _context.RoutineExerciseSets
                .Where(x => DeleteSets.Contains(x.RoutineExerciseSetId))
                .ToListAsync();

            routineExerciseSets.ForEach(x => x.IsActive = false);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteSetsByExercises(List<long> DeleteExercises)
        {
            var routineExerciseSets = await _context.RoutineExerciseSets
                .Include(x => x.IdRoutineExerciseNavigation)
                .Where(x => DeleteExercises.Contains(x.IdRoutineExerciseNavigation.IdExercise))
                .ToListAsync();

            routineExerciseSets.ForEach(x => x.IsActive = false);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
