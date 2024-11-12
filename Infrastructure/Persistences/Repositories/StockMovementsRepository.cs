using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class StockMovementsRepository : GenericRepository<StockMovements>, IStockMovementsRepository
    {
        private readonly DbFithubContext _context;

        public StockMovementsRepository(DbFithubContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterEntryAndExitProduct(StockMovements request)
        {
            _context.StockMovements.Add(request);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
