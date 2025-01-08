using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class UserDeviceTokenRepository : GenericRepository<UserDeviceToken>, IUserDeviceTokenRepository
    {
        private readonly DbFithubContext _context;

        public UserDeviceTokenRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> ExistsUserDeviceToken(string token)
        {
            return await _context.UserDeviceToken.AnyAsync(x => x.Token == token);
        }

        public async Task<List<string>> GetDeviceTokensByAthleteIds(List<int> athleteIds)
        {
            return await _context.UserDeviceToken
                .Where(x => athleteIds.Contains(x.IdAthlete))
                .Select(x => x.Token)
                .ToListAsync();
        }

        public async Task<bool> SaveUserDeviceToken(UserDeviceToken userDeviceToken)
        {
            await _context.UserDeviceToken.AddAsync(userDeviceToken);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
