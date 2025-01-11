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

        public async Task<Routines> GetRoutineById(long routineId)
        {
            var routine = await _context.Routines
                .FirstOrDefaultAsync(x => x.RoutineId == routineId);

            return routine ?? new Routines();
        }

        public async Task<BaseEntityResponse<Routines>> GetRoutinesByAthleteIdList(BaseFiltersRequest filters, int athleteId)
        {
            var response = new BaseEntityResponse<Routines>();

            var routines = _context.Routines
                .Include(m => m.IdMuscleGroupNavigation)
                .Include(re => re.RoutineExercises).ThenInclude(e => e.IdExerciseNavigation)
                .Include(re => re.RoutineExercises).ThenInclude(res => res.RoutineExerciseSets)
                .Include(ae => ae.AthleteRoutines).ThenInclude(a => a.IdAthleteNavigation)
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
                .Include(re => re.RoutineExercises).ThenInclude(e => e.IdExerciseNavigation)
                .Include(re => re.RoutineExercises).ThenInclude(res => res.RoutineExerciseSets)
                .Include(ae => ae.AthleteRoutines).ThenInclude(a => a.IdAthleteNavigation)
                .Where(x => x.IdGym == gymId)
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
