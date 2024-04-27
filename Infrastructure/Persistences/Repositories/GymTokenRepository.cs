using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class GymTokenRepository : GenericRepository<GymToken>, IGymTokenRepository
    {
        private readonly DbFithubContext _context;

        public GymTokenRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int[]> GetGymToken(int gymId)
        {
            return await _context.GymToken
                .Where(x => x.IdGym == gymId && x.Revoked == false)
                .Select(x => x.TokenID)
                .ToArrayAsync();
        }

        public async Task<bool> GetRevokeStatus(string token)
        {
            return await _context.GymToken
                .Where(x => x.Token == token)
                .Select(x => x.Revoked)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> RegisterGymToken(GymToken gymToken)
        {
            await _context.GymToken.AddAsync(gymToken);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RevokeGymToken(string actualToken)
        {
            var token = await _context.GymToken.Where(x => actualToken.Equals(x.Token) && x.Revoked == false).FirstOrDefaultAsync();
            if (token == null)
                return false;

            token.Revoked = true;
            _context.GymToken.Update(token);


            return await _context.SaveChangesAsync() > 0;
        }
    }
}
