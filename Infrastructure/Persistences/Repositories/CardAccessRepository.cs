using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class CardAccessRepository : GenericRepository<CardAccess>, ICardAccessRepository
    {
        private readonly DbFithubContext _context;

        public CardAccessRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<CardAccess> GetAccessCardByCode(string cardCode)
        {
            var cardAccess = await _context.CardAccess
                .Where(x => x.CardNumber.Equals(cardCode))
                .OrderByDescending(x => x.CardId)
                .FirstOrDefaultAsync();

            if (cardAccess == null)
                return null;

            return cardAccess;
        }

        public async Task<bool> GetActiveAccessByCode(string cardCode)
        {
            var cardAccess = await _context.CardAccess
                .Where(x => x.CardNumber.Equals(cardCode) && x.Status.Equals(true))
                .FirstOrDefaultAsync();

            return cardAccess != null;
        }

        public async Task<bool> RegisterCardAccess(CardAccess cardAccess)
        {
            await _context.CardAccess.AddAsync(cardAccess);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }

        public async Task<bool> UnregisterCardAccess(CardAccess cardAccess)
        {
            _context.Update(cardAccess);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
