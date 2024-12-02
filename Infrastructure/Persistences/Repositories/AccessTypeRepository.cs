using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class AccessTypeRepository : GenericRepository<AccessType>, IAccessTypeRepository
    {
        private readonly DbFithubContext _context;

        public AccessTypeRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<AccessType>> ListAccessTypes()
        {
            return await _context.AccessType
                .Where(x => x.Status.Equals(true)).AsNoTracking().ToListAsync();
        }
    }
}
