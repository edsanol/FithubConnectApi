using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbFithubContext _context;

        public IGymRepository GymRepository { get; private set; }

        public UnitOfWork(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            GymRepository = new GymRepository(_context) ?? throw new ArgumentNullException(nameof(GymRepository));
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
