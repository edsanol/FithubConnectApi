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

        public IContactInformationRepository ContactInformationRepository { get; private set; }

        public IAthleteTokenRepository AthleteTokenRepository { get; private set; }

        public IGymTokenRepository GymTokenRepository { get; private set; }

        public IProductsCategoryRepository ProductsCategoryRepository { get; private set; }

        public IProductsRepository ProductsRepository { get; private set; }

        public IProductsVariantRepository ProductsVariantRepository { get; private set; }

        public IStockMovementsRepository StockMovementsRepository { get; private set; }

        public IOrdersPaymentsRepository OrdersPaymentsRepository { get; private set; }

        public IAccessTypeRepository AccessTypeRepository { get; private set; }

        public IGymAccessTypeRepository GymAccessTypeRepository { get; private set; }

        public IChannelRepository ChannelRepository { get; private set; }

        public IChannelUsersRepository ChannelUsersRepository { get; private set; }

        public INotificationRepository NotificationRepository { get; private set; }

        public IUserDeviceTokenRepository UserDeviceTokenRepository { get; private set; }

        public IRoutineRepository RoutineRepository { get; private set; }

        public IExerciseRepository ExerciseRepository { get; private set; }

        public IRoutineExerciseRepository RoutineExerciseRepository { get; private set; }

        public IRoutineExerciseSetsRepository RoutineExerciseSetsRepository { get; private set; }

        public IAthleteRoutineRepository AthleteRoutineRepository { get; private set; }

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
            ContactInformationRepository = new ContactInformationRepository(_context) ?? throw new ArgumentNullException(nameof(ContactInformationRepository));
            AthleteTokenRepository = new AthleteTokenRepository(_context) ?? throw new ArgumentNullException(nameof(AthleteTokenRepository));
            GymTokenRepository = new GymTokenRepository(_context) ?? throw new ArgumentNullException(nameof(GymTokenRepository));
            ProductsCategoryRepository = new ProductsCategoryRepository(_context) ?? throw new ArgumentNullException(nameof(ProductsCategoryRepository));
            ProductsRepository = new ProductsRepository(_context) ?? throw new ArgumentNullException(nameof(ProductsRepository));
            ProductsVariantRepository = new ProductsVariantRepository(_context) ?? throw new ArgumentNullException(nameof(ProductsVariantRepository));
            StockMovementsRepository = new StockMovementsRepository(_context) ?? throw new ArgumentNullException(nameof(StockMovementsRepository));
            OrdersPaymentsRepository = new OrdersPaymentsRepository(_context) ?? throw new ArgumentNullException(nameof(OrdersPaymentsRepository));
            AccessTypeRepository = new AccessTypeRepository(_context) ?? throw new ArgumentNullException(nameof(AccessTypeRepository));
            GymAccessTypeRepository = new GymAccessTypeRepository(_context) ?? throw new ArgumentNullException(nameof(GymAccessTypeRepository));
            ChannelRepository = new ChannelRepository(_context) ?? throw new ArgumentNullException(nameof(ChannelRepository));
            ChannelUsersRepository = new ChannelUsersRepository(_context) ?? throw new ArgumentNullException(nameof(ChannelUsersRepository));
            NotificationRepository = new NotificationRepository(_context) ?? throw new ArgumentNullException(nameof(NotificationRepository));
            UserDeviceTokenRepository = new UserDeviceTokenRepository(_context) ?? throw new ArgumentNullException(nameof(UserDeviceTokenRepository));
            RoutineRepository = new RoutineRepository(_context) ?? throw new ArgumentNullException(nameof(RoutineRepository));
            ExerciseRepository = new ExerciseRepository(_context) ?? throw new ArgumentNullException(nameof(ExerciseRepository));
            RoutineExerciseRepository = new RoutineExerciseRepository(_context) ?? throw new ArgumentNullException(nameof(RoutineExerciseRepository));
            RoutineExerciseSetsRepository = new RoutineExerciseSetsRepository(_context) ?? throw new ArgumentNullException(nameof(RoutineExerciseSetsRepository));
            AthleteRoutineRepository = new AthleteRoutineRepository(_context) ?? throw new ArgumentNullException(nameof(AthleteRoutineRepository));
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
