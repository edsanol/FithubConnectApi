using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class ExerciseRepository : GenericRepository<Exercises>, IExerciseRepository
    {
        private readonly DbFithubContext _context;

        public ExerciseRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CreateExercise(Exercises exercise)
        {
            await _context.Exercises.AddAsync(exercise);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Exercises> GetExerciseById(long exerciseId)
        {
            var response = await _context.Exercises.FirstOrDefaultAsync(x => x.ExerciseId == exerciseId);
            return response ?? throw new Exception("Exercise not found");
        }
    }
}
