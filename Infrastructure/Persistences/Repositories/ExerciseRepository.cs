using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
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

        public async Task<bool> DeleteExercise(long exerciseId)
        {
            var exercise = await _context.Exercises.AsNoTracking().SingleOrDefaultAsync(x => x.ExerciseId == exerciseId);
            if (exercise != null)
            {
                exercise.IsActive = false;
                exercise.UpdatedAt = DateTime.Now;
                _context.Update(exercise);
                var recordsAffected = await _context.SaveChangesAsync();
                return recordsAffected > 0;
            }

            return false;
        }

        public async Task<Exercises> GetExerciseById(long exerciseId)
        {
            var response = await _context.Exercises.FirstOrDefaultAsync(x => x.ExerciseId == exerciseId);
            return response ?? throw new Exception("Exercise not found");
        }

        public async Task<BaseEntityResponse<Exercises>> GetExercisesListByGymId(BaseFiltersRequest filters, int gymId)
        {
            var response = new BaseEntityResponse<Exercises>();

            var exercises = _context.Exercises
                .Include(m => m.IdMuscleGroupNavigation)
                .Where(x => x.IdGym == gymId)
                .AsNoTracking().AsQueryable();

            if (filters.NumFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                var filterTextLower = filters.TextFilter.ToLower();

                switch (filters.NumFilter)
                {
                    case 1:
                        exercises = exercises.Where(x => x.ExerciseTitle.ToLower().Contains(filterTextLower));
                        break;
                    case 2:
                        exercises = exercises.Where(x => x.IdMuscleGroupNavigation.MuscleGroupName.ToLower().Contains(filterTextLower));
                        break;
                    case 3:
                        exercises = exercises.Where(x => x.IdMuscleGroupNavigation.MuscleGroupId.Equals(int.Parse(filters.TextFilter)));
                        break;
                    case 4:
                        exercises = exercises.Where(x => x.ExerciseTitle.ToLower().Contains(filterTextLower) && x.IsActive.Equals(true));
                        break;
                    case 5:
                        exercises = exercises.Where(x => x.IdMuscleGroupNavigation.MuscleGroupName.ToLower().Contains(filterTextLower) 
                            && x.IsActive.Equals(true));
                        break;
                    case 6:
                        exercises = exercises.Where(x => x.IdMuscleGroupNavigation.MuscleGroupId.Equals(Int32.Parse(filters.TextFilter)) 
                            && x.IsActive.Equals(true));
                        break;
                    case 7:
                        if (!string.IsNullOrEmpty(filters.TextFilter))
                        {
                            var exerciseIds = filters.TextFilter
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(long.Parse)
                                .ToList();

                            exercises = exercises.Where(x => exerciseIds.Contains(x.ExerciseId));
                        }
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty(filters.StartDate))
            {
                exercises = exercises.Where(x => x.CreatedAt >= DateTime.Parse(filters.StartDate));
            }

            if (!string.IsNullOrEmpty(filters.EndDate))
            {
                exercises = exercises.Where(x => x.CreatedAt <= DateTime.Parse(filters.EndDate));
            }

            filters.Sort ??= "ExerciseId";
            response.TotalRecords = await exercises.CountAsync();
            response.Items = await Ordering(filters, exercises, !(bool)filters.Download!).ToListAsync();

            return response;
        }

        public async Task<bool> UpdateExercise(Exercises exercise)
        {
            _context.Exercises.Update(exercise);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
