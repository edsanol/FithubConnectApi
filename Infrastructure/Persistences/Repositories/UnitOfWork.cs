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

        public IAthleteMembershipRepository AthleteMembershipRepository { get; private set; }

        public ICardAccessRepository CardAccessRepository { get; private set; }

        public IAccessLogRepository AccessLogRepository { get; private set; }

        public IMeasurementProgressRepository MeasurementProgressRepository { get; private set;}

        public UnitOfWork(DbFithubContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            GymRepository = new GymRepository(_context) ?? throw new ArgumentNullException(nameof(GymRepository));
            AthleteRepository = new AthleteRepository(_context) ?? throw new ArgumentNullException(nameof(AthleteRepository));
            MembershipRepository = new MembershipRepository(_context) ?? throw new ArgumentNullException(nameof(MembershipRepository));
            DiscountRepository = new DiscountRepository(_context) ?? throw new ArgumentNullException(nameof(DiscountRepository));
            AthleteMembershipRepository = new AthleteMembershipRepository(_context) ?? throw new ArgumentNullException(nameof(AthleteMembershipRepository));
            CardAccessRepository = new CardAccessRepository(_context) ?? throw new ArgumentNullException(nameof(CardAccessRepository));
            AccessLogRepository = new AccessLogRepository(_context) ?? throw new ArgumentNullException(nameof(AccessLogRepository));
            MeasurementProgressRepository = new MeasurementProgressRepository(_context) ?? throw new ArgumentNullException(nameof(MeasurementProgressRepository));
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
