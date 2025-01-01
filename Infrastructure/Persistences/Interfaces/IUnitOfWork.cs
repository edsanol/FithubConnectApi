namespace Infrastructure.Persistences.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Declaración o matricula de nuestras interfacez a nivel de repositorio

        IGymRepository GymRepository { get; }
        IAthleteRepository AthleteRepository { get; }
        IMembershipRepository MembershipRepository { get; }
        IDiscountRepository DiscountRepository { get; }
        IAthleteMembershipRepository AthleteMembershipRepository { get; }
        ICardAccessRepository CardAccessRepository { get; }
        IAccessLogRepository AccessLogRepository { get;  }
        IMeasurementProgressRepository MeasurementProgressRepository { get; }
        IContactInformationRepository ContactInformationRepository { get; }
        IAthleteTokenRepository AthleteTokenRepository { get; }
        IGymTokenRepository GymTokenRepository { get; }
        IProductsCategoryRepository ProductsCategoryRepository { get; }
        IProductsRepository ProductsRepository { get; }
        IProductsVariantRepository ProductsVariantRepository { get; }
        IStockMovementsRepository StockMovementsRepository { get; }
        IOrdersPaymentsRepository OrdersPaymentsRepository { get; }
        IAccessTypeRepository AccessTypeRepository { get; }
        IGymAccessTypeRepository GymAccessTypeRepository { get; }
        IChannelRepository ChannelRepository { get; }
        IChannelUsersRepository ChannelUsersRepository { get; }
        INotificationRepository NotificationRepository { get; }

        void SaveChanges();
        Task SaveChangeAsync();
    }
}
