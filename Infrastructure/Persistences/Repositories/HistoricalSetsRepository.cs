using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class HistoricalSetsRepository : GenericRepository<HistoricalSets>, IHistoricalSetsRepository
    {
        private readonly DbFithubContext _context;

        public HistoricalSetsRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> InsertHistoricalSets(HistoricalSets historicalSets)
        {
            await _context.HistoricalSets.AddAsync(historicalSets);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
