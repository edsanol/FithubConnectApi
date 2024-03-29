using Domain.Entities;
using Infrastructure.Commons.Bases.Request;
using Infrastructure.Commons.Bases.Response;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class GymRepository : GenericRepository<Gym>, IGymRepository
    {
        private readonly DbFithubContext _context;

        public GymRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<BaseEntityResponse<Gym>> ListGyms(BaseFiltersRequest filters)
        {
            var response = new BaseEntityResponse<Gym>();

            var gyms = (from c in _context.Gym
                        select c).AsNoTracking().AsQueryable();

            if (filters.NumFilter is not null && !string.IsNullOrEmpty(filters.TextFilter))
            {
                switch (filters.NumFilter)
                {
                    case 1:
                        gyms = gyms.Where(x => x.GymName.Contains(filters.TextFilter));
                        break;
                    case 2:
                        gyms = gyms.Where(x => x.Nit.Contains(filters.TextFilter));
                        break;
                }
            }

            if (filters.StateFilter is not null)
            {
                gyms = gyms.Where(x => x.Status.Equals(filters.StateFilter));
            }

            if (!string.IsNullOrEmpty(filters.StartDate))
            {
                gyms = gyms.Where(x => Convert.ToDateTime(x.RegisterDate) >= Convert.ToDateTime(filters.StartDate));
            }

            if (filters.Sort is null) filters.Sort = "GymId";

            response.TotalRecords = await gyms.CountAsync();
            response.Items = await Ordering(filters, gyms, !(bool)filters.Download!).ToListAsync();

            return response;
        }

        public async Task<IEnumerable<Gym>> ListSelectGyms()
        {
            var gyms = await _context.Gym
                .Where(x => x.Status.Equals(true)).AsNoTracking().ToListAsync();

            return gyms;
        }

        public async Task<Gym> GetGymById(int gymID)
        {
            var gym = await _context.Gym.AsNoTracking().FirstOrDefaultAsync(x => x.GymId.Equals(gymID));

            return gym!;
        }

        public async Task<bool> RegisterGym(Gym gym)
        {
            await _context.AddAsync(gym);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }

        public async Task<bool> EditGym(Gym gym)
        {
            _context.Update(gym);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }

        public async Task<bool> DeleteGym(int gymID)
        {
            var gym = await _context.Gym.AsNoTracking().SingleOrDefaultAsync(x => x.GymId.Equals(gymID));
            gym.Status = false;
            gym.RegisterDate = Convert.ToDateTime(gym.RegisterDate);
            _context.Update(gym);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;

        }

        public async Task<Gym> LoginGym(string email)
        {
            var gym = await _context.Gym.AsNoTracking().FirstOrDefaultAsync(x => x.Email.Equals(email));

            return gym!;
        }

        public async Task<bool> ResetPasswordAsync(int gymId, string newPassword)
        {
            var gym = await _context.Gym.AsNoTracking().FirstOrDefaultAsync(x => x.GymId.Equals(gymId));
            gym.Password = newPassword;
            _context.Update(gym);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> ChangePasswordAsync(int gymId, string newPassword)
        {
            var gym = await _context.Gym.AsNoTracking().FirstOrDefaultAsync(x => x.GymId.Equals(gymId));
            gym.Password = newPassword;
            _context.Update(gym);
            var recordsAffected = await _context.SaveChangesAsync();
            return recordsAffected > 0;
        }

        public async Task<bool> HasAthleteByAthleteID(int gymID, int athleteID)
        {
            var gym = await _context.Gym.AsNoTracking().Include(x => x.Athletes).FirstOrDefaultAsync(x => x.GymId.Equals(gymID));
            var athlete = gym.Athletes.FirstOrDefault(x => x.AthleteId.Equals(athleteID));
            return athlete != null;
        }
    }
}
