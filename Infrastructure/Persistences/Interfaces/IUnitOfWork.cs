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

        void SaveChanges();
        Task SaveChangeAsync();
    }
}
