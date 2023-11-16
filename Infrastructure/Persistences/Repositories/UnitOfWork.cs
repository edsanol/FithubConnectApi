using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;

namespace Infrastructure.Persistences.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbFithubContext _context;

        public IGymRepository GymRepository { get; private set; }

        public IAthleteRepository AthleteRepository { get; private set; }

        public IMembershipRepository MembershipRepository { get; private set; }

        public IDiscountRepository DiscountRepository { get; private set;}

        public UnitOfWork(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            GymRepository = new GymRepository(_context) ?? throw new ArgumentNullException(nameof(GymRepository));
            AthleteRepository = new AthleteRepository(_context) ?? throw new ArgumentNullException(nameof(AthleteRepository));
            MembershipRepository = new MembershipRepository(_context) ?? throw new ArgumentNullException(nameof(MembershipRepository));
            DiscountRepository = new DiscountRepository(_context) ?? throw new ArgumentNullException(nameof(DiscountRepository));
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
