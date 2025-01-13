using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> DeleteRoutine(long exerciseId)
        {
            var routine = await _context.Routines.AsNoTracking()
                .SingleOrDefaultAsync(x => x.RoutineId == exerciseId);

            if (routine is not null)
            {
                routine.IsActive = false;
                routine.UpdatedAt = DateTime.Now;
                _context.Update(routine);
                var recordsAffected = await _context.SaveChangesAsync();
                return recordsAffected > 0;
            }

            return false;
        }

        public async Task<Routines> GetRoutineById(long routineId)
        {
            var routine = await _context.Routines
                .FirstOrDefaultAsync(x => x.RoutineId == routineId);

            return routine ?? new Routines();
        }

        public async Task<Routines> GetRoutineByRoutineId(long routineId)
        {
            var routines = _context.Routines
                .Include(m => m.IdMuscleGroupNavigation)
                .Include(re => re.RoutineExercises.Where(re => re.IsActive))
                    .ThenInclude(e => e.IdExerciseNavigation)
                .Include(re => re.RoutineExercises)
                    .ThenInclude(res => res.RoutineExerciseSets.Where(res => res.IsActive))
                .Include(ae => ae.AthleteRoutines)
                    .ThenInclude(a => a.IdAthleteNavigation)
                    .AsNoTracking().AsQueryable();

            return await routines.FirstOrDefaultAsync(x => x.RoutineId == routineId)
                ?? new Routines();
        }

        public async Task<BaseEntityResponse<Routines>> GetRoutinesByAthleteIdList(BaseFiltersRequest filters, int athleteId)
        {
            var response = new BaseEntityResponse<Routines>();

            var routines = _context.Routines
                .Include(m => m.IdMuscleGroupNavigation)
                .Include(re => re.RoutineExercises.Where(re => re.IsActive))
                    .ThenInclude(e => e.IdExerciseNavigation)
                .Include(re => re.RoutineExercises)
                    .ThenInclude(res => res.RoutineExerciseSets.Where(res => res.IsActive))
                .Include(ae => ae.AthleteRoutines)
                    .ThenInclude(a => a.IdAthleteNavigation)
                .Where(x => x.AthleteRoutines.Any(a => a.IdAthlete == athleteId))
                .AsNoTracking().AsQueryable();

            if (filters.NumFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                var filterTextLower = filters.TextFilter.ToLower();

                switch (filters.NumFilter)
                {
                    case 1:
                        routines = routines.Where(x => x.Title.ToLower().Contains(filterTextLower));
                        break;
                    case 2:
                        routines = routines.Where(x => x.IdMuscleGroupNavigation.MuscleGroupName.ToLower().Contains(filterTextLower));
                        break;
                    case 3:
                        routines = routines.Where(x => x.IdMuscleGroupNavigation.MuscleGroupId.Equals(Int32.Parse(filters.TextFilter)));
                        break;
                    case 4:
                        routines = routines.Where(x => x.Title.ToLower().Contains(filterTextLower) && x.IsActive.Equals(true));
                        break;
                    case 5:
                        routines = routines.Where(x => x.IdMuscleGroupNavigation.MuscleGroupName.ToLower().Contains(filterTextLower)
                                                   && x.IsActive.Equals(true));
                        break;
                    case 6:
                        routines = routines.Where(x => x.IdMuscleGroupNavigation.MuscleGroupId.Equals(Int32.Parse(filters.TextFilter))
                                                   && x.IsActive.Equals(true));
                        break;
                    case 7:
                        routines = routines.Where(x => x.AthleteRoutines.Any(ar =>
                            !ar.Status.Equals("Inactive", StringComparison.OrdinalIgnoreCase)) && x.IsActive.Equals(true));
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty(filters.StartDate))
            {
                routines = routines.Where(x => x.CreatedAt >= DateTime.Parse(filters.StartDate));
            }

            if (!string.IsNullOrEmpty(filters.EndDate))
            {
                routines = routines.Where(x => x.CreatedAt <= DateTime.Parse(filters.EndDate));
            }

            filters.Sort ??= "RoutineId";
            response.TotalRecords = await routines.CountAsync();
            response.Items = await Ordering(filters, routines, !(bool)filters.Download!).ToListAsync();

            return response;
        }

        public async Task<BaseEntityResponse<Routines>> GetRoutinesListByGymId(BaseFiltersRequest filters, int gymId)
        {
            var response = new BaseEntityResponse<Routines>();

            var routines = _context.Routines
                .Include(m => m.IdMuscleGroupNavigation)
                .Include(re => re.RoutineExercises.Where(re => re.IsActive))
                    .ThenInclude(e => e.IdExerciseNavigation)
                .Include(re => re.RoutineExercises)
                    .ThenInclude(res => res.RoutineExerciseSets.Where(res => res.IsActive))
                .Include(ae => ae.AthleteRoutines)
                    .ThenInclude(a => a.IdAthleteNavigation)
                .Where(x => x.IdGym == gymId 
                    && x.RoutineExercises.Any(re => re.IdExerciseNavigation.IsActive))
                .AsNoTracking().AsQueryable();

            if (filters.NumFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                var filterTextLower = filters.TextFilter.ToLower();

                switch (filters.NumFilter)
                {
                    case 1:
                        routines = routines.Where(x => x.Title.ToLower().Contains(filterTextLower));
                        break;
                    case 2:
                        routines = routines.Where(x => x.IdMuscleGroupNavigation.MuscleGroupName.ToLower().Contains(filterTextLower));
                        break;
                    case 3:
                        routines = routines.Where(x => x.IdMuscleGroupNavigation.MuscleGroupId.Equals(Int32.Parse(filters.TextFilter)));
                        break;
                    case 4:
                        routines = routines.Where(x => x.Title.ToLower().Contains(filterTextLower) && x.IsActive.Equals(true));
                        break;
                    case 5:
                        routines = routines.Where(x => x.IdMuscleGroupNavigation.MuscleGroupName.ToLower().Contains(filterTextLower)
                            && x.IsActive.Equals(true));
                        break;
                    case 6:
                        routines = routines.Where(x => x.IdMuscleGroupNavigation.MuscleGroupId.Equals(Int32.Parse(filters.TextFilter))
                            && x.IsActive.Equals(true));
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty(filters.StartDate))
            {
                routines = routines.Where(x => x.CreatedAt >= DateTime.Parse(filters.StartDate));
            }

            if (!string.IsNullOrEmpty(filters.EndDate))
            {
                routines = routines.Where(x => x.CreatedAt <= DateTime.Parse(filters.EndDate));
            }

            filters.Sort ??= "RoutineId";
            response.TotalRecords = await routines.CountAsync();
            response.Items = await Ordering(filters, routines, !(bool)filters.Download!).ToListAsync();

            return response;
        }

        public async Task<bool> UpdateRoutine(Routines routine)
        {
            _context.Routines.Update(routine);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
