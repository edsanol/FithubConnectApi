using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class AccessLogRepository : GenericRepository<AccessLog>, IAccessLogRepository
    {
        private readonly DbFithubContext _context;

        public AccessLogRepository(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> RegisterAccessLog(AccessLog accessLog)
        {
            await _context.AccessLog.AddAsync(accessLog);
            var recordsAffected = await _context.SaveChangesAsync();

            return recordsAffected > 0;
        }
    }
}
