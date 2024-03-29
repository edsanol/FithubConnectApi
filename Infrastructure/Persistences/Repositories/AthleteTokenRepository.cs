using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class AthleteTokenRepository : GenericRepository<AthleteToken>, IAthleteTokenRepository
    {
        private readonly DbFithubContext _context;

        public AthleteTokenRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int[]> GetAthleteToken(int athleteId)
        {
            return await _context.AthleteToken
                .Where(x => x.IdAthlete == athleteId && x.Revoked == false)
                .Select(x => x.TokenID)
                .ToArrayAsync();
        }

        public async Task<bool> GetRevokeStatus(string token)
        {
            return await _context.AthleteToken
                .Where(x => x.Token == token)
                .Select(x => x.Revoked)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> RegisterAthleteToken(AthleteToken athleteToken)
        {
            await _context.AthleteToken.AddAsync(athleteToken);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RevokeAthleteToken(int[] tokenID)
        {
            var tokens = await _context.AthleteToken.Where(x => tokenID.Contains(x.TokenID)).ToListAsync();
            if (tokens == null)
                return false;

            foreach (var token in tokens)
            {
                token.Revoked = true;
                _context.AthleteToken.Update(token);
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
